CREATE DATABASE  IF NOT EXISTS `phenologydb` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `phenologydb`;
-- MySQL dump 10.13  Distrib 5.6.17, for Win32 (x86)
--
-- Host: localhost    Database: phenologydb
-- ------------------------------------------------------
-- Server version	5.6.19

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cr_raw_clim`
--

DROP TABLE IF EXISTS `cr_raw_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cr_raw_clim` (
  `LST_DATE` date NOT NULL,
  `WBANNO` varchar(5) NOT NULL,
  `CRX_VN` float DEFAULT NULL,
  `LONGITUDE` double(8,3) DEFAULT NULL,
  `LATITUDE` double(8,3) DEFAULT NULL,
  `T_DAILY_MAX` double(6,2) DEFAULT NULL,
  `T_DAILY_MIN` double(6,2) DEFAULT NULL,
  `T_DAILY_MEAN` double(6,2) DEFAULT NULL,
  `T_DAILY_AVG` double(6,2) DEFAULT NULL,
  `P_DAILY_CALC` double(6,2) DEFAULT NULL,
  `SOLARAD_DAILY` double(6,2) DEFAULT NULL,
  `SUR_TEMP_DAILY_TYPE` varchar(1) NOT NULL,
  `SUR_TEMP_DAILY_MAX` double(6,2) DEFAULT NULL,
  `SUR_TEMP_DAILY_MIN` double(6,2) DEFAULT NULL,
  `SUR_TEMP_DAILY_AVG` double(6,2) DEFAULT NULL,
  `RH_DAILY_MAX` double(6,2) DEFAULT NULL,
  `RH_DAILY_MIN` double(6,2) DEFAULT NULL,
  `RH_DAILY_AVG` double(6,2) DEFAULT NULL,
  `SOIL_MOISTURE_5_DAILY` double(6,2) DEFAULT NULL,
  `SOIL_MOISTURE_10_DAILY` double(6,2) DEFAULT NULL,
  `SOIL_MOISTURE_20_DAILY` double(6,2) DEFAULT NULL,
  `SOIL_MOISTURE_50_DAILY` double(6,2) DEFAULT NULL,
  `SOIL_MOISTURE_100_DAILY` double(6,2) DEFAULT NULL,
  `SOIL_TEMP_5_DAILY` double(6,2) DEFAULT NULL,
  `SOIL_TEMP_10_DAILY` double(6,2) DEFAULT NULL,
  `SOIL_TEMP_20_DAILY` double(6,2) DEFAULT NULL,
  `SOIL_TEMP_50_DAILY` double(6,2) DEFAULT NULL,
  `SOIL_TEMP_100_DAILY` double(6,2) DEFAULT NULL,
  PRIMARY KEY (`LST_DATE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cr_std_clim`
--

DROP TABLE IF EXISTS `cr_std_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cr_std_clim` (
  `DATE` date NOT NULL,
  `SITE_CODE` varchar(2) NOT NULL,
  `T_DAILY_MIN` double DEFAULT '-99.99',
  `T_DAILY_MAX` double DEFAULT '-99.99',
  `T_DAILY_MEAN` double DEFAULT '-99.99',
  `T_DAILY_AVG` double DEFAULT '-99.99',
  `PPT_DAILY_MM` double DEFAULT '-99.99',
  `RH_DAILY_MIN` double DEFAULT '-99.99',
  `RH_DAILY_MAX` double DEFAULT '-99.99',
  `RH_DAILY_MEAN` double DEFAULT '-99.99',
  `VPD_DAILY_AVG` double DEFAULT '-99.99',
  `AGDD` double DEFAULT '-99.99',
  `DAY_LENGTH` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_100_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_10_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_20_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_50_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_100_DAILY_BARE` double DEFAULT '-99.99',
  PRIMARY KEY (`DATE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dailysolarnoonvis`
--

DROP TABLE IF EXISTS `dailysolarnoonvis`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dailysolarnoonvis` (
  `CameraName` varchar(10) NOT NULL DEFAULT '',
  `SiteCode` varchar(2) DEFAULT NULL,
  `ROI` varchar(10) NOT NULL,
  `X` double DEFAULT NULL,
  `Y` double DEFAULT NULL,
  `W` double DEFAULT NULL,
  `H` double DEFAULT NULL,
  `Idx_Label` double DEFAULT NULL,
  `RedDN` double DEFAULT NULL,
  `GreenDN` double DEFAULT NULL,
  `BlueDN` double DEFAULT NULL,
  `pctR` double DEFAULT NULL,
  `pctG` double DEFAULT NULL,
  `pctB` double DEFAULT NULL,
  `nndvi` double DEFAULT NULL,
  `totalRGB` double DEFAULT NULL,
  `DATE` date NOT NULL DEFAULT '0000-00-00',
  `Img` varchar(30) DEFAULT NULL,
  `2G_Rbi` double DEFAULT NULL,
  PRIMARY KEY (`CameraName`,`DATE`,`ROI`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gi_rawclim`
--

DROP TABLE IF EXISTS `gi_rawclim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gi_rawclim` (
  `DATE` date NOT NULL DEFAULT '0000-00-00',
  `WS_AVG_MS` double DEFAULT NULL,
  `WINDDIR_DEG` double DEFAULT NULL,
  `T_AVG_C` double DEFAULT NULL,
  `RH_MAX_PREC` double DEFAULT NULL,
  `PPT-MM` double DEFAULT NULL,
  `DAY_LENGTH` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`DATE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gi_std_clim`
--

DROP TABLE IF EXISTS `gi_std_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gi_std_clim` (
  `DATE` date NOT NULL,
  `SITE_CODE` varchar(2) NOT NULL,
  `T_DAILY_MIN` double DEFAULT '-99.99',
  `T_DAILY_MAX` double DEFAULT '-99.99',
  `T_DAILY_MEAN` double DEFAULT '-99.99',
  `T_DAILY_AVG` double DEFAULT '-99.99',
  `PPT_DAILY_MM` double DEFAULT '-99.99',
  `RH_DAILY_MIN` double DEFAULT '-99.99',
  `RH_DAILY_MAX` double DEFAULT '-99.99',
  `RH_DAILY_MEAN` double DEFAULT '-99.99',
  `VPD_DAILY_AVG` double DEFAULT '-99.99',
  `AGDD` double DEFAULT '-99.99',
  `DAY_LENGTH` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_30_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_30_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_35_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_SHRUB` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_SHRUB` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_SHRUB` double DEFAULT '-99.99',
  `SOIL_MOISTURE_30_DAILY_SHRUB` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_SHRUB` double DEFAULT '-99.99',
  `inserted_temp_data` tinyint(1) DEFAULT '0',
  `inserted_moist_data` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`DATE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `p9_datadaily`
--

DROP TABLE IF EXISTS `p9_datadaily`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `p9_datadaily` (
  `DATE` date NOT NULL,
  `TIME` time NOT NULL,
  `T_DAILY_MIN` double DEFAULT NULL,
  `T_DAILY_MAX` double DEFAULT NULL,
  `T_DAILY_AVG` double DEFAULT NULL,
  `DAY_LENGTH` double DEFAULT NULL,
  PRIMARY KEY (`DATE`,`TIME`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `p9_dataraindaily`
--

DROP TABLE IF EXISTS `p9_dataraindaily`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `p9_dataraindaily` (
  `DATE` date NOT NULL,
  `TIME` time NOT NULL,
  `PPT_DAILY_MM` double DEFAULT NULL,
  PRIMARY KEY (`DATE`,`TIME`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `p9_std_clim`
--

DROP TABLE IF EXISTS `p9_std_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `p9_std_clim` (
  `DATE` date NOT NULL,
  `SITE_CODE` varchar(2) NOT NULL,
  `T_DAILY_MIN` double DEFAULT '-99.99',
  `T_DAILY_MAX` double DEFAULT '-99.99',
  `T_DAILY_MEAN` double DEFAULT '-99.99',
  `T_DAILY_AVG` double DEFAULT '-99.99',
  `PPT_DAILY_MM` double DEFAULT '-99.99',
  `GI_RH_DAILY_MIN` double DEFAULT '-99.99',
  `GI_RH_DAILY_MAX` double DEFAULT '-99.99',
  `GI_RH_DAILY_MEAN` double DEFAULT '-99.99',
  `VPD_DAILY_AVG` double DEFAULT '-99.99',
  `AGDD` double DEFAULT '-99.99',
  `DAY_LENGTH` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_70_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_60_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_SHRUB` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_SHRUB` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_SHRUB` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_SHRUB` double DEFAULT '-99.99',
  `SOIL_MOISTURE_70_DAILY_SHRUB` double DEFAULT '-99.99',
  `has_gi_rh_data` tinyint(1) DEFAULT '0',
  `inserted_temp_data` tinyint(1) DEFAULT '0',
  `inserted_moist_data` tinyint(1) DEFAULT '0',
  `inserted_ppt_data` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`DATE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sc_std_clim`
--

DROP TABLE IF EXISTS `sc_std_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sc_std_clim` (
  `DATE` date NOT NULL,
  `SITE_CODE` varchar(2) NOT NULL,
  `T_DAILY_MIN` double DEFAULT '-99.99',
  `T_DAILY_MAX` double DEFAULT '-99.99',
  `T_DAILY_MEAN` double DEFAULT '-99.99',
  `T_DAILY_AVG` double DEFAULT '-99.99',
  `PPT_DAILY_MM` double DEFAULT '-99.99',
  `RH_DAILY_MIN` double DEFAULT '-99.99',
  `RH_DAILY_MAX` double DEFAULT '-99.99',
  `RH_DAILY_MEAN` double DEFAULT '-99.99',
  `VPD_DAILY_AVG` double DEFAULT '-99.99',
  `AGDD` double DEFAULT '-99.99',
  `DAY_LENGTH` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_100_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_100_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_100_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_5_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_TEMP_10_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_TEMP_20_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_TEMP_50_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_TEMP_100_DAILY_DUNE` double DEFAULT '-99.99',
  `SOIL_TEMP_5_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_TEMP_10_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_TEMP_20_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_TEMP_50_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_TEMP_100_DAILY_GRASS` double DEFAULT '-99.99',
  `SOIL_TEMP_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_10_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_20_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_50_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_100_DAILY_BARE` double DEFAULT '-99.99',
  PRIMARY KEY (`DATE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `siteinfo`
--

DROP TABLE IF EXISTS `siteinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `siteinfo` (
  `SITECODE` varchar(2) NOT NULL,
  `SITENAME` varchar(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tr_raw_clim`
--

DROP TABLE IF EXISTS `tr_raw_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tr_raw_clim` (
  `DATETIME` datetime NOT NULL,
  `SOIL_MOISTURE_5CM` double DEFAULT NULL,
  `SOIL_MOISTURE_15CM` double DEFAULT NULL,
  `SOIL_MOISTURE_30CM` double DEFAULT NULL,
  `SOIL_MOISTURE_50CM` double DEFAULT NULL,
  `AIR_TEMP_DEG` double(7,4) DEFAULT NULL,
  `RH_AVG` double(7,4) DEFAULT NULL,
  `PPT_TOTAL` double DEFAULT NULL,
  `SOIL_TEMP_5CM` double(7,4) DEFAULT NULL,
  `SOIL_TEMP_15CM` double(7,4) DEFAULT NULL,
  `SOIL_TEMP_30CM` double(7,4) DEFAULT NULL,
  `SOIL_TEMP_50CM` double(7,4) DEFAULT NULL,
  PRIMARY KEY (`DATETIME`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tr_std_clim`
--

DROP TABLE IF EXISTS `tr_std_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tr_std_clim` (
  `DATE` date NOT NULL,
  `SITE_CODE` varchar(2) NOT NULL,
  `T_DAILY_MIN` double DEFAULT '-99.99',
  `T_DAILY_MAX` double DEFAULT '-99.99',
  `T_DAILY_MEAN` double DEFAULT '-99.99',
  `T_DAILY_AVG` double DEFAULT '-99.99',
  `PPT_DAILY_MM` double DEFAULT '-99.99',
  `RH_DAILY_MIN` double DEFAULT '-99.99',
  `RH_DAILY_MAX` double DEFAULT '-99.99',
  `RH_DAILY_MEAN` double DEFAULT '-99.99',
  `VPD_DAILY_AVG` double DEFAULT '-99.99',
  `AGDD` double DEFAULT '-99.99',
  `DAY_LENGTH` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_15_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_30_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_50_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_15_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_30_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_50_DAILY_BARE` double DEFAULT '-99.99',
  PRIMARY KEY (`DATE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tw_raw_moistemp_clim`
--

DROP TABLE IF EXISTS `tw_raw_moistemp_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tw_raw_moistemp_clim` (
  `DATE` date NOT NULL,
  `HOUR` date NOT NULL,
  `SOIL_MOISTURE_2CM_HOURLY_BARE` double DEFAULT NULL,
  `SOIL_MOISTURE_5CM_HOURLY_BARE` double DEFAULT NULL,
  `SOIL_MOISTURE_10CM_HOURLY_BARE` double DEFAULT NULL,
  `SOIL_MOISTURE_20CM_HOURLY_BARE` double DEFAULT NULL,
  `SOIL_TEMP_2CM_HOURLY_BARE` double DEFAULT NULL,
  `SOIL_TEMP_5CM_HOURLY_BARE` double DEFAULT NULL,
  `SOIL_TEMP_10CM_HOURLY_BARE` double DEFAULT NULL,
  `SOIL_TEMP_20CM_HOURLY_BARE` double DEFAULT NULL,
  PRIMARY KEY (`DATE`,`HOUR`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tw_raw_temppt_clim`
--

DROP TABLE IF EXISTS `tw_raw_temppt_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tw_raw_temppt_clim` (
  `DATE` date NOT NULL,
  `T_DAILY_MIN` double DEFAULT NULL,
  `T_DAILY_MAX` double DEFAULT NULL,
  `T_DAILY_MEAN` double DEFAULT NULL,
  `T_DAILY_AVG` double DEFAULT NULL,
  `PPT_DAILY` double DEFAULT NULL,
  `RH_DAILY_MIN` double DEFAULT NULL,
  `RH_DAILY_MAX` double DEFAULT NULL,
  `RH_DAILY_MEAN` double DEFAULT NULL,
  `VPD_DAILY_AVG` double DEFAULT NULL,
  PRIMARY KEY (`DATE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tw_std_clim`
--

DROP TABLE IF EXISTS `tw_std_clim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tw_std_clim` (
  `Date` date NOT NULL,
  `SITE_CODE` varchar(2) NOT NULL,
  `T_DAILY_MIN` double DEFAULT '-99.99',
  `T_DAILY_MAX` double DEFAULT '-99.99',
  `T_DAILY_MEAN` double DEFAULT '-99.99',
  `T_DAILY_AVG` double DEFAULT '-99.99',
  `PPT_DAILY_MM` double DEFAULT '-99.99',
  `RH_DAILY_MIN` double DEFAULT '-99.99',
  `RH_DAILY_MAX` double DEFAULT '-99.99',
  `RH_DAILY_MEAN` double DEFAULT '-99.99',
  `VPD_DAILY_AVG` double DEFAULT '-99.99',
  `AGDD` double DEFAULT '-99.99',
  `DAY_LENGTH` double DEFAULT '-99.99',
  `SOIL_MOISTURE_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_10_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_MOISTURE_20_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_5_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_10_DAILY_BARE` double DEFAULT '-99.99',
  `SOIL_TEMP_20_DAILY_BARE` double DEFAULT '-99.99',
  `inserted_temp_data` tinyint(1) DEFAULT '0',
  `inserted_moist_data` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`Date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2014-08-21 13:31:18
