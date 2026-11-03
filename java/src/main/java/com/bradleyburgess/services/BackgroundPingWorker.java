package com.bradleyburgess.services;

import java.time.Instant;
import java.time.Duration;

import com.bradleyburgess.config.PingConfig;
import com.bradleyburgess.entities.Outage;
import com.bradleyburgess.entities.PingResult;
import com.bradleyburgess.enums.PingResultType;
import com.bradleyburgess.interfaces.IPinger;

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

    @Scheduled(every = "{ping.interval-seconds}")
    @Transactional
    public void periodicPing() {
        var pingResult = pinger.ping();
        var previousSuccessTime = updatePingResults(pingResult);
        updateOutages(pingResult, previousSuccessTime);
    }

    private Instant updatePingResults(boolean pingWasSuccess) {
        PingResult mostRecentPing = PingResult.findByType(PingResultType.MostRecent);
        PingResult mostRecentSuccessPing = PingResult.findByType(PingResultType.MostRecentSuccess);
        Instant previousSuccessTime = mostRecentSuccessPing.lastDate;
        var now = Instant.now();
        mostRecentPing.lastDate = now;
        if (pingWasSuccess) {

            mostRecentPing.success = true;
            mostRecentSuccessPing.success = true;
            mostRecentSuccessPing.lastDate = now;
        } else {
            mostRecentPing.success = false;
        }
        return previousSuccessTime;
    }

    private void updateOutages(boolean wasPingSuccess, Instant lastSuccessTime) {
        var currentOutage = Outage.findCurrentOngoing();
        var now = Instant.now();
        if (wasPingSuccess) {
            if (currentOutage.isPresent()) {
                currentOutage.get().endDate = Instant.now();
            } else if (lastSuccessTime != null
                    && lastSuccessTime.plus(Duration.ofSeconds(pingConfig.thresholdSeconds())).isBefore(now)) {
                var newOutage = new Outage();
                newOutage.startDate = lastSuccessTime;
                newOutage.persist();
            }
        } else {
            if (currentOutage.isEmpty()) {
                var newOutage = new Outage();
                newOutage.startDate = Instant.now();
                newOutage.persist();
            }
        }
    }
}
