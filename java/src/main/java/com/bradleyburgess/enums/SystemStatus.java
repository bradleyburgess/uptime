package com.bradleyburgess.enums;

import com.fasterxml.jackson.annotation.JsonValue;

public enum SystemStatus {
    ONLINE("online"),
    OFFLINE("offline");

    private final String value;

    SystemStatus(String value) {
        this.value = value;
    }

    @JsonValue
    public String getValue() {
        return value;
    }
}
