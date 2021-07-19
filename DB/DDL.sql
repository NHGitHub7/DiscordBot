DROP TABLE IF EXISTS version;
CREATE TABLE version (
  version_id INTEGER AUTO_INCREMENT,
  name VARCHAR(30) NOT NULL,
  version VARCHAR(255) NOT NULL,
  PRIMARY KEY (version_id)
);

INSERT INTO version(name, version) VALUES ("discord_bot", "2021061400");

DROP TABLE IF EXISTS customcommands;
CREATE TABLE customcommands (
  CustomCommandId integer(11) NOT NULL AUTO_INCREMENT,
  CommandName varchar(100),
  CommandResponse varchar(2000),
  DateCreated datetime,
  CreatedBy varchar(100),
  ModifiedBy varchar(100),
  DateModified datetime,
  PRIMARY KEY (CustomCommandId)
);

DROP TABLE IF EXISTS customroles;
CREATE TABLE customroles(
  RoleID integer(11) NOT NULL AUTO_INCREMENT,
  rolename varchar(255),
  password varchar(255),
  PRIMARY KEY (RoleID)
);

DROP TABLE IF EXISTS swearwords;
CREATE TABLE swearwords(
  swearwords_id INTEGER AUTO_INCREMENT,
  word VARCHAR(255),
  UNIQUE(word),
  PRIMARY KEY (swearwords_id)
);

INSERT INTO swearwords(word)
VALUES  ("scheisse"),
        ("shit"),
        ("fuck");

DROP TABLE IF EXISTS swearword_strikes;
CREATE TABLE swearword_strikes(
  user_id BIGINT UNSIGNED,
  strikes INTEGER,
  PRIMARY KEY(user_id)
);
