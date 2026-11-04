package com.bradleyburgess.rest;

import com.bradleyburgess.config.PingConfig;
import com.bradleyburgess.dtos.CurrentStatusDto;
import com.bradleyburgess.dtos.OutageDto;
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
        var outages = Outage.getMostRecent().stream()
                .map(OutageDto::from)
                .toList();

        var systemStatus = Outage.findCurrentOngoing().isPresent()
                ? SystemStatus.OFFLINE
                : SystemStatus.ONLINE;

        var lastSuccess = PingResult.findByType(PingResultType.MostRecentSuccess);
        var currentStatus = new CurrentStatusDto(
                systemStatus,
                lastSuccess == null ? null : lastSuccess.lastDate,
                pingConfig.host());
        return new UptimeDto(outages, currentStatus);
    }
}
