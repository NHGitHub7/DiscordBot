CREATE TABLE version (
  version_id INTEGER AUTO_INCREMENT,
  version INTEGER NOT NULL,
  last_updated TIMESTAMP NOT NULL
    DEFAULT CURRENT_TIMESTAMP
    ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (version_id)
);

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