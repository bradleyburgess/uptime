package com.bradleyburgess.dtos;

import java.time.Duration;
import java.time.Instant;

import com.bradleyburgess.entities.Outage;

public record OutageDto(Instant startDate, Instant endDate, Duration duration) {

        public static OutageDto from(Outage outage) {
                return new OutageDto(outage.startDate, outage.endDate,
                                Duration.between(outage.startDate, outage.endDate));
        }
}
