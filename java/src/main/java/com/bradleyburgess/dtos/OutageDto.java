package com.bradleyburgess.dtos;

import java.time.Duration;
import java.time.Instant;

import io.smallrye.common.constraint.NotNull;
import io.smallrye.common.constraint.Nullable;

public record OutageDto(
        @NotNull Instant startTime,
        @Nullable Instant endTime,
        @Nullable Duration duration) {
}
