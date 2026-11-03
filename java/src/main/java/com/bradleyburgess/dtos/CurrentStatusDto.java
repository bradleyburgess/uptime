package com.bradleyburgess.dtos;

import java.time.Instant;

import com.bradleyburgess.enums.SystemStatus;

import io.smallrye.common.constraint.NotNull;
import io.smallrye.common.constraint.Nullable;

public record CurrentStatusDto(
        @NotNull SystemStatus status,
        @Nullable Instant lastSuccessfulPing,
        @NotNull String monitoringHost) {
}