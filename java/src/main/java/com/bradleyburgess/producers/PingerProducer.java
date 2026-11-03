package com.bradleyburgess.producers;

import org.eclipse.microprofile.rest.client.inject.RestClient;

import com.bradleyburgess.config.PingConfig;
import com.bradleyburgess.interfaces.IPinger;
import com.bradleyburgess.services.HttpPinger;
import com.bradleyburgess.services.IcmpPinger;
import com.bradleyburgess.services.PingClient;

import jakarta.enterprise.context.ApplicationScoped;
import jakarta.enterprise.inject.Instance;
import jakarta.enterprise.inject.Produces;
import jakarta.inject.Inject;

@ApplicationScoped
public class PingerProducer {
    @Inject
    PingConfig pingConfig;

    @Produces
    @ApplicationScoped
    IPinger pinger(PingConfig config, @RestClient Instance<PingClient> client) {
        String host = config.host();
        boolean http = host.startsWith("http://") || host.startsWith("https://");
        return http ? new HttpPinger(client.get()) : new IcmpPinger(host);
    }
}
