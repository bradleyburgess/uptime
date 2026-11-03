package com.bradleyburgess.dtos;

import java.util.List;

import com.bradleyburgess.entities.Outage;

public record UptimeDto(
        List<Outage> outages,
        CurrentStatusDto currentStatus) {

}
