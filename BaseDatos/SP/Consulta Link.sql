SELECT SUM(SUMA) AS TOTAL
FROM
(SELECT COUNT(RESPONSE_CODE) AS SUMA from dbo.transactionsales where CONVERT(date, date_time)=CONVERT(date,GETDATE()-1)
UNION ALL
SELECT COUNT(RESPONSE_CODE) AS SUMA from APOLO2.TenServ_Celex.dbo.transactionsales where CONVERT(date, date_time)=CONVERT(date,GETDATE()-1)
UNION ALL
SELECT COUNT(RESPONSE_CODE) AS SUMA from APOLO3.TenServ_Celex.dbo.transactionsales where CONVERT(date, date_time)=CONVERT(date,GETDATE()-1)
UNION ALL
SELECT COUNT(RESPONSE_CODE) AS SUMA from APOLO4.TenServ_Celex.dbo.transactionsales where CONVERT(date, date_time)=CONVERT(date,GETDATE()-1)
UNION ALL
SELECT COUNT(RESPONSE_CODE) AS SUMA from APOLO5.TenServ_Celex.dbo.transactionsales where CONVERT(date, date_time)=CONVERT(date,GETDATE()-1)
)AS RESULTADO