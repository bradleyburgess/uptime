package com.bradleyburgess.entities;

import java.time.Instant;

import com.bradleyburgess.enums.PingResultType;

import io.quarkus.hibernate.orm.panache.PanacheEntityBase;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

@Entity(name = "pingResult")
@Table(name = "ping_result")

public class PingResult extends PanacheEntityBase {
    @Id
    @Column(name = "type")
    @Enumerated(EnumType.STRING)
    public PingResultType type;

    @Column(name = "success")
    public Boolean success;

    @Column(name = "last_date")
    public Instant lastDate;

    public static PingResult findByType(PingResultType type) {
        return find("type", type).firstResult();
    }
}
