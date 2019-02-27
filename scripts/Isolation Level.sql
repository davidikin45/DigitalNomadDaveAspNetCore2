SELECT CASE transaction_isolation_level
WHEN 0  THEN 'Unspecified'
WHEN 1 THEN 'ReadUncommitted'
WHEN 2 THEN 'ReadCommitted'
WHEN 3 THEN 'Repeatable'
WHEN 4 THEN 'Serializable'
WHEN 5 THEN 'Snapshot' END AS TRANSACTION_ISOLATION_LEVEL
FROM sys.dm_exec_sessions
where session_id = @@SPID

SELECT name, is_read_committed_snapshot_on 
FROM sys.databases

ALTER DATABASE DND SET READ_COMMITTED_SNAPSHOT ON