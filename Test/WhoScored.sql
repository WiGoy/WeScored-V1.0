SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

CREATE SCHEMA IF NOT EXISTS `WhoScoredDB` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci ;
USE `WhoScoredDB` ;

-- -----------------------------------------------------
-- Table `WhoScoredDB`.`Team`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `WhoScoredDB`.`Team` (
  `ID` INT NOT NULL,
  `Name` VARCHAR(20) NULL,
  `League` VARCHAR(20) NULL,
  `HomePlayed` INT NULL,
  `HomeWin` INT NULL,
  `HomeDraw` INT NULL,
  `HomeLoss` INT NULL,
  `HomeGoalsFor` INT NULL,
  `HomeGoalsAgainst` INT NULL,
  `Points` INT NULL,
  `AwayPlayed` INT NULL,
  `AwayWin` INT NULL,
  `AwayDraw` INT NULL,
  `AwayLoss` INT NULL,
  `AwayGoalsFor` INT NULL,
  `AwayGoalsAgainst` INT NULL,
  `AwayPoints` INT NULL,
  `OverallPlayed` INT NULL,
  `OverallWin` INT NULL,
  `OverallDraw` INT NULL,
  `OverallLoss` INT NULL,
  `OverallGoalsFor` INT NULL,
  `HomeGoalsAgainst_copy2` INT NULL,
  `OverallPoints` INT NULL,
  PRIMARY KEY (`ID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `WhoScoredDB`.`PlayerRating`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `WhoScoredDB`.`PlayerRating` (
  `ID` INT NOT NULL,
  `MatchID` INT NULL,
  `PlayerID` INT NULL,
  `Rating` FLOAT NULL,
  PRIMARY KEY (`ID`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
