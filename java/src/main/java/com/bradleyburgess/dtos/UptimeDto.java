package com.bradleyburgess.dtos;

import java.util.List;

public record UptimeDto(
        List<OutageDto> outages,
        CurrentStatusDto currentStatus) {
}
