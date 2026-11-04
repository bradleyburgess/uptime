package com.bradleyburgess.dtos;

import java.time.Instant;

import com.bradleyburgess.enums.SystemStatus;

public record CurrentStatusDto(
                SystemStatus status,
                Instant lastSuccessfulPing,
                String monitoringHost) {
}