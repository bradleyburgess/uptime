package com.bradleyburgess.config;

import io.smallrye.config.ConfigMapping;
import io.smallrye.config.WithDefault;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Positive;

@ConfigMapping(prefix = "ping")
public interface PingConfig {
    @NotBlank
    @WithDefault("1.1.1.1")
    String host();

    @Positive
    @WithDefault("5")
    int intervalSeconds();

    @Positive
    @WithDefault("20")
    Integer thresholdSeconds();
}
