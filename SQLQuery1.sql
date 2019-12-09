select * from t_Monitor_info

use tab_ESD_MonitorDB
select top 10 * from t_ESD_status_record order by t_Creation_time desc

--drop table Device
--create table Device(
--  D_ID int identity(1,1) primary key,
--   D_Creadte_Time datetime,
--   D_Line nvarchar(20),
--   D_Monitor nvarchar(20),
--   D_Name nvarchar(18) not null,
--   D_Adress nvarchar(18)not null,
--   D_Yieds int,
--  D_Bednum int,
--   D_Power_Time decimal(10,2),
--   D_Power decimal(10,2),
--   D_Computer_Name nvarchar(20),
--   D_IP_adress nvarchar(20),
--   D_MAC_adress nvarchar(50)
--)

select MAX( D_Yieds) as D_Yieds ,D_Name from Device group by D_Name

select * from Device

