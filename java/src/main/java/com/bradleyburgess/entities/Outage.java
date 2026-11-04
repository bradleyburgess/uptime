package com.bradleyburgess.entities;

import java.time.Instant;
import java.util.List;
import java.util.Optional;

import io.quarkus.hibernate.orm.panache.PanacheEntityBase;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

@Entity(name = "outage")
@Table(name = "outage")
public class Outage extends PanacheEntityBase {
    @Column(name = "id", nullable = false, columnDefinition = "INTEGER")
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    public Long id;

    @Column(name = "start_date", nullable = false)
    public Instant startDate;

    @Column(name = "end_date")
    public Instant endDate;

    public static Optional<Outage> findCurrentOngoing() {
        return Optional.ofNullable(find("endDate IS NULL").firstResult());
    }

    public static List<Outage> getMostRecent(int limit) {
        return find("order by startDate desc").page(0, limit).list();
    }

    public static List<Outage> getMostRecent() {
        return getMostRecent(10);
    }
}
