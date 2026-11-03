package com.bradleyburgess.config;

import io.smallrye.config.ConfigMapping;

@ConfigMapping(prefix = "ping")
public interface PingConfig {
    String host();

    Integer intervalSeconds();

    Integer thresholdSeconds();
}
