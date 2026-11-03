package com.bradleyburgess.rest;

import java.util.List;

import com.bradleyburgess.config.PingConfig;
import com.bradleyburgess.dtos.CurrentStatusDto;
import com.bradleyburgess.dtos.UptimeDto;
import com.bradleyburgess.dtos.VersionDto;
import com.bradleyburgess.entities.Outage;
import com.bradleyburgess.entities.PingResult;
import com.bradleyburgess.enums.PingResultType;
import com.bradleyburgess.enums.SystemStatus;

import io.quarkus.info.BuildInfo;
import jakarta.inject.Inject;
import jakarta.ws.rs.GET;
import jakarta.ws.rs.Path;

@Path("/api")
public class Resources {
    @Inject
    BuildInfo info;

    @Inject
    PingConfig pingConfig;

    @GET
    @Path("version")
    public VersionDto getVersion() {
        return new VersionDto(info.version());
    }

    @GET
    @Path("current-status")
    public UptimeDto getCurrentStatus() {
        List<Outage> outages = Outage.getMostRecent();
        List<PingResult> pingResults = PingResult.listAll();

        var systemStatus = outages.getFirst().endDate == null
                ? SystemStatus.OFFLINE
                : SystemStatus.ONLINE;

        var lastSuccessfulPing = pingResults.stream()
                .filter(p -> p.type == PingResultType.MostRecentSuccess)
                .findFirst()
                .orElse(null);

        var currentStatusDto = new CurrentStatusDto(systemStatus, lastSuccessfulPing.lastDate, pingConfig.host());
        return new UptimeDto(outages, currentStatusDto);
    }
}
