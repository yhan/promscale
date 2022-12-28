
-- minute by minute compare


In finance, we often need to identify outliers of a certain indicator
for ex: we need to compare hour by hour the sum of traded nominal to the one of last 15 days.
To experiment this, I am playing with the sample data provided by promscale: https://github.com/timescale/promscale


-- SHOW POSTGRES EXTENSIONS
SELECT e.extname AS "Name", e.extversion AS "Version", n.nspname AS
"Schema", c.description AS "Description"
FROM pg_catalog.pg_extension e
LEFT JOIN pg_catalog.pg_namespace n
ON n.oid = e.extnamespace
LEFT JOIN pg_catalog.pg_description c
ON c.objoid = e.oid AND c.classoid =
'pg_catalog.pg_extension'::pg_catalog.regclass
ORDER BY 1;


---------------------------------
-- POC

select * from _prom_catalog.series where id=3686;
select * from _prom_catalog.label where id in(1117, 202,242,258,262,264);
select * from _prom_catalog.label where value='Nominal';


select * from _prom_catalog.metric where id=187;
select * from prom_metric.calls_total;

select * from prom_metric.go_gc_duration_seconds where instance_id=156 and job_id=157 and quantile_id=299;

select distinct val(instance_id), val(job_id)  from go_memstats_heap_alloc_bytes
   WHERE "time" > now() - INTERVAL '3 hours'
        AND "time" < now() - INTERVAL '1 hours';


select count(0) from prom_metric.go_gc_duration_seconds;
select * from _prom_catalog.series where id=482;


select k, round(percentile_disc(k) within group (order by "go_memstats_heap_alloc_bytes".value)), count(0)
from "go_memstats_heap_alloc_bytes", generate_series(0.1, 1, 0.1) as k
where "time" > now() - INTERVAL '1 days'
group by k;


SELECT * from go_memstats_heap_sys_bytes;

SELECT count(0) from go_memstats_heap_alloc_bytes;

select
val(operation_id) as operation,
val(service_name_id) as service_name,
sum(value)
from calls_total
group by operation_id, service_name_id;
----------------------------------------------------------------------------------------------------------------


SELECT * FROM prom_metric.foo;
SELECT count(0) from prom_metric."Nominal";

SELECT * from prom_metric."Nominal";

SELECT prom_api.drop_metric('foo');


--===========================================================
-- HOUR BY HOUR COMPARE
-- today
select extract(hour from hour::timestamp) AS time, avg("sum") from (
SELECT time_bucket('1 hours', "time") AS "hour",
             sum(value)                        AS "sum"
      FROM "Nominal"
      WHERE "time" > now() - INTERVAL '1 days'
      group by "time"
      order by "time") t
group by time
order by time;
-- last 15d
select extract(hour from hour::timestamp) AS time, avg("sum") AS last_15d from (
SELECT time_bucket('1 hours', "time") AS "hour",
             sum(value)                        AS "sum"
      FROM "Nominal"
      WHERE "time" > now() - INTERVAL '15 days'
      AND "time" < now() - INTERVAL '1 days'
      group by "time"
      order by "time") t
group by time
order by time;
--===========================================================

  SELECT
  val("StrategyName_id") as "Strategy",
  val("VenueCategory_id") as "VenueCategory",
  sum(value)
FROM "Nominal"
--WHERE  $__timeFilter("time")
  group by "StrategyName_id","VenueCategory_id";

select round(avg(value)::numeric, 0), count(0),  til from(
select val("StrategyName_id"),
       value,
       ntile(90) over (order by value) as til
from prom_metric."Nominal" where val("StrategyName_id")='Hit'
                           --and  "time" > now() - INTERVAL '1 days'
                            AND $__timeFilter("time")
                           ) as foo
group by til;


----------------------------------------------------------------------------------
select
       bucket,
       round(avg(value)::numeric, 0) avg_in_bucket,
       --round(min(value)::numeric, 0) min_in_bucket,
       round(max(value)::numeric, 0) max_in_bucket,
       count(0)
from (select            value,
             ntile(10) over (order by value) as bucket
      from prom_metric."Nominal"
      where
          --val("StrategyName_id") = 'Hit' and
          "time" > now() - INTERVAL '1 days'
         --AND $__timeFilter("time")
     ) as foo
group by bucket
order by bucket;


select k as percentile,
       round(percentile_disc(k) within group (order by "Nominal".value)) as disc,
       count(0)
from "Nominal", generate_series(0.1, 1, 0.1) as k
where "time" > now() - INTERVAL '1 days'
group by k;
----------------------------------------------------------------------------------

select
  percentile_disc(0.1) within group (order by prom_metric."Nominal".value),
  percentile_disc(0.2) within group (order by prom_metric."Nominal".value),
  percentile_disc(0.3) within group (order by prom_metric."Nominal".value),
  count(0)
from prom_metric."Nominal"
where "time" > now() - INTERVAL '1 days';


  SELECT
  val("StrategyName_id") as "Strategy",
  val("VenueCategory_id") as "VenueCategory",
  sum(value)
FROM "Nominal"
WHERE
 --$__timeFilter("time")
"time" > now() - INTERVAL '1 days'
  group by "StrategyName_id","VenueCategory_id";


select count(0) from "Nominal";

select to_char(125, '999');
select to_char(1, '99');


select
       --strat,
       to_char(bucket*10, '999') || '%',
       round(avg(value)::numeric, 0) avg_in_bucket
       --round(min(value)::numeric, 0) min_in_bucket,
       --round(max(value)::numeric, 0) max_in_bucket,
       --count(0)
from (select --val("StrategyName_id") strat,
             value,
             ntile(10) over (order by value) as bucket
      from prom_metric."Nominal"
      where
          --val("StrategyName_id") = 'Hit' and
          "time" > now() - INTERVAL '1 days'
         --AND $__timeFilter("time")
     ) as foo
group by /*strat,*/ bucket
order by bucket;


select time, value,
       val("Counterparty_id"),
       val("VenueCategory_id"),
       val("VenueType_id"),
       val("VenueType_id"),
       val("StrategyName_id"),
       val("TopLevelStrategyName_id")
from "Nominal";


-- TIME FILTER
WHERE
 $__timeFilter("time")  
 