package com.bradleyburgess.services;

import org.eclipse.microprofile.rest.client.inject.RegisterRestClient;
import org.jboss.resteasy.reactive.RestResponse;

import jakarta.ws.rs.HEAD;

@RegisterRestClient(configKey = "ping")
public interface PingClient {

    @HEAD
    RestResponse<Void> ping();
}
