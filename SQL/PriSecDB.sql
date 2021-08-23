-- phpMyAdmin SQL Dump
-- version 4.9.5deb2
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: Aug 23, 2021 at 06:54 AM
-- Server version: 8.0.26-0ubuntu0.20.04.2
-- PHP Version: 7.4.21

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `PriSecDB`
--

-- --------------------------------------------------------

--
-- Table structure for table `Account_Lock`
--

CREATE TABLE `Account_Lock` (
  `Payment_ID` varchar(500) NOT NULL,
  `ED25519_PK` text NOT NULL,
  `Signed_ED25519_PK` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `Account_Lock`
--

-- --------------------------------------------------------

--
-- Table structure for table `Payment`
--

CREATE TABLE `Payment` (
  `Expiration_Date` datetime DEFAULT NULL,
  `Bytes_Used` text,
  `ID` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `Payment`
--

-- --------------------------------------------------------

--
-- Table structure for table `Random_Challenge`
--

CREATE TABLE `Random_Challenge` (
  `Challenge` text NOT NULL,
  `Valid_Duration` datetime DEFAULT NULL,
  `ID` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `Random_Challenge`
--

-- --------------------------------------------------------

--
-- Table structure for table `SDH`
--

CREATE TABLE `SDH` (
  `Payment_ID` varchar(500) NOT NULL,
  `Signed_X25519_PK` text NOT NULL,
  `ED25519_PK` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `SDH`
--

-- --------------------------------------------------------

--
-- Table structure for table `X3SDH`
--

CREATE TABLE `X3SDH` (
  `Payment_ID` varchar(500) NOT NULL,
  `SPK_Signed_X25519_PK` text NOT NULL,
  `SPK_ED25519_PK` text NOT NULL,
  `IK_Signed_X25519_PK` text NOT NULL,
  `IK_ED25519_PK` text NOT NULL,
  `OPK_Signed_X25519_PK` text NOT NULL,
  `OPK_ED25519_PK` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `X3SDH`
--

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Account_Lock`
--
ALTER TABLE `Account_Lock`
  ADD UNIQUE KEY `Payment_ID` (`Payment_ID`);

--
-- Indexes for table `Payment`
--
ALTER TABLE `Payment`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `Random_Challenge`
--
ALTER TABLE `Random_Challenge`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `SDH`
--
ALTER TABLE `SDH`
  ADD UNIQUE KEY `Payment_ID` (`Payment_ID`);

--
-- Indexes for table `X3SDH`
--
ALTER TABLE `X3SDH`
  ADD UNIQUE KEY `Payment_ID` (`Payment_ID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
