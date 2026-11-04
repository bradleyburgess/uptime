package com.bradleyburgess.services;

import java.time.Instant;
import java.time.Duration;

import com.bradleyburgess.config.PingConfig;
import com.bradleyburgess.entities.Outage;
import com.bradleyburgess.entities.PingResult;
import com.bradleyburgess.enums.PingResultType;
import com.bradleyburgess.interfaces.IPinger;

import io.quarkus.narayana.jta.QuarkusTransaction;
import io.quarkus.scheduler.Scheduled;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.inject.Inject;
import jakarta.transaction.Transactional;

@ApplicationScoped
public class BackgroundPingWorker {
    @Inject
    IPinger pinger;

    @Inject
    PingConfig pingConfig;

    @Scheduled(every = "{ping.interval-seconds}", concurrentExecution = Scheduled.ConcurrentExecution.SKIP)
    public void periodicPing() {
        var up = pinger.ping();
        var now = Instant.now();
        QuarkusTransaction.requiringNew().run(() -> record(up, now));
    }

    private void record(boolean up, Instant now) {
        var previousSuccessTime = updatePingResults(up, now);
        updateOutages(up, now, previousSuccessTime);
    }

    private Instant updatePingResults(boolean up, Instant now) {
        PingResult mostRecentPing = PingResult.findByType(PingResultType.MostRecent);
        PingResult mostRecentSuccessPing = PingResult.findByType(PingResultType.MostRecentSuccess);
        Instant previousSuccessTime = mostRecentSuccessPing.lastDate;
        mostRecentPing.lastDate = now;
        if (up) {
            mostRecentPing.success = true;
            mostRecentSuccessPing.success = true;
            mostRecentSuccessPing.lastDate = now;
        } else {
            mostRecentPing.success = false;
        }
        return previousSuccessTime;
    }

    private void updateOutages(boolean up, Instant now, Instant lastSuccessTime) {
        var currentOutage = Outage.findCurrentOngoing();
        if (up) {
            if (currentOutage.isPresent()) {
                currentOutage.get().endDate = now;
            } else if (isPastThreshold(lastSuccessTime, now)) {
                var newOutage = new Outage();
                newOutage.startDate = lastSuccessTime;
                newOutage.endDate = now;
                newOutage.persist();
            }
        } else if (currentOutage.isEmpty() && isPastThreshold(lastSuccessTime, now)) {
            var newOutage = new Outage();
            newOutage.startDate = now;
            newOutage.persist();
        }
    }

    private boolean isPastThreshold(Instant lastSuccessTime, Instant now) {
        return lastSuccessTime != null
                && lastSuccessTime
                        .plus(Duration.ofSeconds(pingConfig.thresholdSeconds()))
                        .isBefore(now);
    }
}
