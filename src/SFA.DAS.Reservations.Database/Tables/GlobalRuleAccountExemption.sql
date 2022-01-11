CREATE TABLE [dbo].[GlobalRuleAccountExemption]
(
	[GlobalRuleId] BIGINT NOT NULL , 
    [AccountId] BIGINT NOT NULL, 
    CONSTRAINT [PK_GlobalRuleAccountExemption] PRIMARY KEY ([GlobalRuleId], [AccountId]),
    CONSTRAINT [FK_GlobalRuleAccountExemption_GlobalRule] FOREIGN KEY (GlobalRuleId) REFERENCES [GlobalRule]([Id]),
    CONSTRAINT [FK_GlobalRuleAccountExemption_Account] FOREIGN KEY (AccountId) REFERENCES [Account]([Id])
)
