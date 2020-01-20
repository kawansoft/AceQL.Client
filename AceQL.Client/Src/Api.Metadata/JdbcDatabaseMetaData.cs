/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2020,  KawanSoft SAS
 * (http://www.kawansoft.com). All rights reserved.                                
 *                                                                               
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Api.Metadata
{
    /// <summary>
    /// Class JdbcDatabaseMetaData. An instance contains main SQL meta info sent by the remote JDBC Driver.
    /// The info matches the JDBC <a href="https://docs.oracle.com/javase/8/docs/api/java/sql/DatabaseMetaData.html">DatabaseMetaData</a> main values.
    /// </summary>
    public class JdbcDatabaseMetaData
    {
        private String getURL;
        private bool isReadOnly;
        private bool allProceduresAreCallable;
        private bool allTablesAreSelectable;
        private String getUserName;
        private bool nullsAreSortedHigh;
        private bool nullsAreSortedLow;
        private bool nullsAreSortedAtStart;
        private bool nullsAreSortedAtEnd;
        private String getDatabaseProductName;
        private String getDatabaseProductVersion;
        private String getDriverName;
        private String getDriverVersion;
        private int getDriverMajorVersion;
        private int getDriverMinorVersion;
        private bool usesLocalFiles;
        private bool usesLocalFilePerTable;
        private bool supportsMixedCaseIdentifiers;
        private bool storesUpperCaseIdentifiers;
        private bool storesLowerCaseIdentifiers;
        private bool storesMixedCaseIdentifiers;
        private bool supportsMixedCaseQuotedIdentifiers;
        private bool storesUpperCaseQuotedIdentifiers;
        private bool storesLowerCaseQuotedIdentifiers;
        private bool storesMixedCaseQuotedIdentifiers;
        private String getIdentifierQuoteString;
        private String getSQLKeywords;
        private String getNumericFunctions;
        private String getStringFunctions;
        private String getSystemFunctions;
        private String getTimeDateFunctions;
        private String getSearchStringEscape;
        private String getExtraNameCharacters;
        private bool supportsAlterTableWithAddColumn;
        private bool supportsAlterTableWithDropColumn;
        private bool supportsColumnAliasing;
        private bool nullPlusNonNullIsNull;
        private bool supportsConvert;
        private bool supportsTableCorrelationNames;
        private bool supportsDifferentTableCorrelationNames;
        private bool supportsExpressionsInOrderBy;
        private bool supportsOrderByUnrelated;
        private bool supportsGroupBy;
        private bool supportsGroupByUnrelated;
        private bool supportsGroupByBeyondSelect;
        private bool supportsLikeEscapeClause;
        private bool supportsMultipleResultSets;
        private bool supportsMultipleTransactions;
        private bool supportsNonNullableColumns;
        private bool supportsMinimumSQLGrammar;
        private bool supportsCoreSQLGrammar;
        private bool supportsExtendedSQLGrammar;
        private bool supportsANSI92EntryLevelSQL;
        private bool supportsANSI92IntermediateSQL;
        private bool supportsANSI92FullSQL;
        private bool supportsIntegrityEnhancementFacility;
        private bool supportsOuterJoins;
        private bool supportsFullOuterJoins;
        private bool supportsLimitedOuterJoins;
        private String getSchemaTerm;
        private String getProcedureTerm;
        private String getCatalogTerm;
        private bool isCatalogAtStart;
        private String getCatalogSeparator;
        private bool supportsSchemasInDataManipulation;
        private bool supportsSchemasInProcedureCalls;
        private bool supportsSchemasInTableDefinitions;
        private bool supportsSchemasInIndexDefinitions;
        private bool supportsSchemasInPrivilegeDefinitions;
        private bool supportsCatalogsInDataManipulation;
        private bool supportsCatalogsInProcedureCalls;
        private bool supportsCatalogsInTableDefinitions;
        private bool supportsCatalogsInIndexDefinitions;
        private bool supportsCatalogsInPrivilegeDefinitions;
        private bool supportsPositionedDelete;
        private bool supportsPositionedUpdate;
        private bool supportsSelectForUpdate;
        private bool supportsStoredProcedures;
        private bool supportsSubqueriesInComparisons;
        private bool supportsSubqueriesInExists;
        private bool supportsSubqueriesInIns;
        private bool supportsSubqueriesInQuantifieds;
        private bool supportsCorrelatedSubqueries;
        private bool supportsUnion;
        private bool supportsUnionAll;
        private bool supportsOpenCursorsAcrossCommit;
        private bool supportsOpenCursorsAcrossRollback;
        private bool supportsOpenStatementsAcrossCommit;
        private bool supportsOpenStatementsAcrossRollback;
        private int getMaxBinaryLiteralLength;
        private int getMaxCharLiteralLength;
        private int getMaxColumnNameLength;
        private int getMaxColumnsInGroupBy;
        private int getMaxColumnsInIndex;
        private int getMaxColumnsInOrderBy;
        private int getMaxColumnsInSelect;
        private int getMaxColumnsInTable;
        private int getMaxConnections;
        private int getMaxCursorNameLength;
        private int getMaxIndexLength;
        private int getMaxSchemaNameLength;
        private int getMaxProcedureNameLength;
        private int getMaxCatalogNameLength;
        private int getMaxRowSize;
        private bool doesMaxRowSizeIncludeBlobs;
        private int getMaxStatementLength;
        private int getMaxStatements;
        private int getMaxTableNameLength;
        private int getMaxTablesInSelect;
        private int getMaxUserNameLength;
        private int getDefaultTransactionIsolation;
        private bool supportsTransactions;
        private bool supportsDataDefinitionAndDataManipulationTransactions;
        private bool supportsDataManipulationTransactionsOnly;
        private bool dataDefinitionCausesTransactionCommit;
        private bool dataDefinitionIgnoredInTransactions;
        private bool supportsBatchUpdates;
        private bool supportsSavepoints;
        private bool supportsNamedParameters;
        private bool supportsMultipleOpenResults;
        private bool supportsGetGeneratedKeys;
        private int getDatabaseMajorVersion;
        private int getDatabaseMinorVersion;
        private int getJDBCMajorVersion;
        private int getJDBCMinorVersion;
        private int getSQLStateType;
        private bool locatorsUpdateCopy;
        private bool supportsStatementPooling;
        private bool supportsStoredFunctionsUsingCallSyntax;
        private bool autoCommitFailureClosesAllResultSets;
        private int getResultSetHoldability;

        public string GetURL { get => getURL; set => getURL = value; }
        public bool IsReadOnly { get => isReadOnly; set => isReadOnly = value; }
        public bool AllProceduresAreCallable { get => allProceduresAreCallable; set => allProceduresAreCallable = value; }
        public bool AllTablesAreSelectable { get => allTablesAreSelectable; set => allTablesAreSelectable = value; }
        public string GetUserName { get => getUserName; set => getUserName = value; }
        public bool NullsAreSortedHigh { get => nullsAreSortedHigh; set => nullsAreSortedHigh = value; }
        public bool NullsAreSortedLow { get => nullsAreSortedLow; set => nullsAreSortedLow = value; }
        public bool NullsAreSortedAtStart { get => nullsAreSortedAtStart; set => nullsAreSortedAtStart = value; }
        public bool NullsAreSortedAtEnd { get => nullsAreSortedAtEnd; set => nullsAreSortedAtEnd = value; }
        public string GetDatabaseProductName { get => getDatabaseProductName; set => getDatabaseProductName = value; }
        public string GetDatabaseProductVersion { get => getDatabaseProductVersion; set => getDatabaseProductVersion = value; }
        public string GetDriverName { get => getDriverName; set => getDriverName = value; }
        public string GetDriverVersion { get => getDriverVersion; set => getDriverVersion = value; }
        public int GetDriverMajorVersion { get => getDriverMajorVersion; set => getDriverMajorVersion = value; }
        public int GetDriverMinorVersion { get => getDriverMinorVersion; set => getDriverMinorVersion = value; }
        public bool UsesLocalFiles { get => usesLocalFiles; set => usesLocalFiles = value; }
        public bool UsesLocalFilePerTable { get => usesLocalFilePerTable; set => usesLocalFilePerTable = value; }
        public bool SupportsMixedCaseIdentifiers { get => supportsMixedCaseIdentifiers; set => supportsMixedCaseIdentifiers = value; }
        public bool StoresUpperCaseIdentifiers { get => storesUpperCaseIdentifiers; set => storesUpperCaseIdentifiers = value; }
        public bool StoresLowerCaseIdentifiers { get => storesLowerCaseIdentifiers; set => storesLowerCaseIdentifiers = value; }
        public bool StoresMixedCaseIdentifiers { get => storesMixedCaseIdentifiers; set => storesMixedCaseIdentifiers = value; }
        public bool SupportsMixedCaseQuotedIdentifiers { get => supportsMixedCaseQuotedIdentifiers; set => supportsMixedCaseQuotedIdentifiers = value; }
        public bool StoresUpperCaseQuotedIdentifiers { get => storesUpperCaseQuotedIdentifiers; set => storesUpperCaseQuotedIdentifiers = value; }
        public bool StoresLowerCaseQuotedIdentifiers { get => storesLowerCaseQuotedIdentifiers; set => storesLowerCaseQuotedIdentifiers = value; }
        public bool StoresMixedCaseQuotedIdentifiers { get => storesMixedCaseQuotedIdentifiers; set => storesMixedCaseQuotedIdentifiers = value; }
        public string GetIdentifierQuoteString { get => getIdentifierQuoteString; set => getIdentifierQuoteString = value; }
        public string GetSQLKeywords { get => getSQLKeywords; set => getSQLKeywords = value; }
        public string GetNumericFunctions { get => getNumericFunctions; set => getNumericFunctions = value; }
        public string GetStringFunctions { get => getStringFunctions; set => getStringFunctions = value; }
        public string GetSystemFunctions { get => getSystemFunctions; set => getSystemFunctions = value; }
        public string GetTimeDateFunctions { get => getTimeDateFunctions; set => getTimeDateFunctions = value; }
        public string GetSearchStringEscape { get => getSearchStringEscape; set => getSearchStringEscape = value; }
        public string GetExtraNameCharacters { get => getExtraNameCharacters; set => getExtraNameCharacters = value; }
        public bool SupportsAlterTableWithAddColumn { get => supportsAlterTableWithAddColumn; set => supportsAlterTableWithAddColumn = value; }
        public bool SupportsAlterTableWithDropColumn { get => supportsAlterTableWithDropColumn; set => supportsAlterTableWithDropColumn = value; }
        public bool SupportsColumnAliasing { get => supportsColumnAliasing; set => supportsColumnAliasing = value; }
        public bool NullPlusNonNullIsNull { get => nullPlusNonNullIsNull; set => nullPlusNonNullIsNull = value; }
        public bool SupportsConvert { get => supportsConvert; set => supportsConvert = value; }
        public bool SupportsTableCorrelationNames { get => supportsTableCorrelationNames; set => supportsTableCorrelationNames = value; }
        public bool SupportsDifferentTableCorrelationNames { get => supportsDifferentTableCorrelationNames; set => supportsDifferentTableCorrelationNames = value; }
        public bool SupportsExpressionsInOrderBy { get => supportsExpressionsInOrderBy; set => supportsExpressionsInOrderBy = value; }
        public bool SupportsOrderByUnrelated { get => supportsOrderByUnrelated; set => supportsOrderByUnrelated = value; }
        public bool SupportsGroupBy { get => supportsGroupBy; set => supportsGroupBy = value; }
        public bool SupportsGroupByUnrelated { get => supportsGroupByUnrelated; set => supportsGroupByUnrelated = value; }
        public bool SupportsGroupByBeyondSelect { get => supportsGroupByBeyondSelect; set => supportsGroupByBeyondSelect = value; }
        public bool SupportsLikeEscapeClause { get => supportsLikeEscapeClause; set => supportsLikeEscapeClause = value; }
        public bool SupportsMultipleResultSets { get => supportsMultipleResultSets; set => supportsMultipleResultSets = value; }
        public bool SupportsMultipleTransactions { get => supportsMultipleTransactions; set => supportsMultipleTransactions = value; }
        public bool SupportsNonNullableColumns { get => supportsNonNullableColumns; set => supportsNonNullableColumns = value; }
        public bool SupportsMinimumSQLGrammar { get => supportsMinimumSQLGrammar; set => supportsMinimumSQLGrammar = value; }
        public bool SupportsCoreSQLGrammar { get => supportsCoreSQLGrammar; set => supportsCoreSQLGrammar = value; }
        public bool SupportsExtendedSQLGrammar { get => supportsExtendedSQLGrammar; set => supportsExtendedSQLGrammar = value; }
        public bool SupportsANSI92EntryLevelSQL { get => supportsANSI92EntryLevelSQL; set => supportsANSI92EntryLevelSQL = value; }
        public bool SupportsANSI92IntermediateSQL { get => supportsANSI92IntermediateSQL; set => supportsANSI92IntermediateSQL = value; }
        public bool SupportsANSI92FullSQL { get => supportsANSI92FullSQL; set => supportsANSI92FullSQL = value; }
        public bool SupportsIntegrityEnhancementFacility { get => supportsIntegrityEnhancementFacility; set => supportsIntegrityEnhancementFacility = value; }
        public bool SupportsOuterJoins { get => supportsOuterJoins; set => supportsOuterJoins = value; }
        public bool SupportsFullOuterJoins { get => supportsFullOuterJoins; set => supportsFullOuterJoins = value; }
        public bool SupportsLimitedOuterJoins { get => supportsLimitedOuterJoins; set => supportsLimitedOuterJoins = value; }
        public string GetSchemaTerm { get => getSchemaTerm; set => getSchemaTerm = value; }
        public string GetProcedureTerm { get => getProcedureTerm; set => getProcedureTerm = value; }
        public string GetCatalogTerm { get => getCatalogTerm; set => getCatalogTerm = value; }
        public bool IsCatalogAtStart { get => isCatalogAtStart; set => isCatalogAtStart = value; }
        public string GetCatalogSeparator { get => getCatalogSeparator; set => getCatalogSeparator = value; }
        public bool SupportsSchemasInDataManipulation { get => supportsSchemasInDataManipulation; set => supportsSchemasInDataManipulation = value; }
        public bool SupportsSchemasInProcedureCalls { get => supportsSchemasInProcedureCalls; set => supportsSchemasInProcedureCalls = value; }
        public bool SupportsSchemasInTableDefinitions { get => supportsSchemasInTableDefinitions; set => supportsSchemasInTableDefinitions = value; }
        public bool SupportsSchemasInIndexDefinitions { get => supportsSchemasInIndexDefinitions; set => supportsSchemasInIndexDefinitions = value; }
        public bool SupportsSchemasInPrivilegeDefinitions { get => supportsSchemasInPrivilegeDefinitions; set => supportsSchemasInPrivilegeDefinitions = value; }
        public bool SupportsCatalogsInDataManipulation { get => supportsCatalogsInDataManipulation; set => supportsCatalogsInDataManipulation = value; }
        public bool SupportsCatalogsInProcedureCalls { get => supportsCatalogsInProcedureCalls; set => supportsCatalogsInProcedureCalls = value; }
        public bool SupportsCatalogsInTableDefinitions { get => supportsCatalogsInTableDefinitions; set => supportsCatalogsInTableDefinitions = value; }
        public bool SupportsCatalogsInIndexDefinitions { get => supportsCatalogsInIndexDefinitions; set => supportsCatalogsInIndexDefinitions = value; }
        public bool SupportsCatalogsInPrivilegeDefinitions { get => supportsCatalogsInPrivilegeDefinitions; set => supportsCatalogsInPrivilegeDefinitions = value; }
        public bool SupportsPositionedDelete { get => supportsPositionedDelete; set => supportsPositionedDelete = value; }
        public bool SupportsPositionedUpdate { get => supportsPositionedUpdate; set => supportsPositionedUpdate = value; }
        public bool SupportsSelectForUpdate { get => supportsSelectForUpdate; set => supportsSelectForUpdate = value; }
        public bool SupportsStoredProcedures { get => supportsStoredProcedures; set => supportsStoredProcedures = value; }
        public bool SupportsSubqueriesInComparisons { get => supportsSubqueriesInComparisons; set => supportsSubqueriesInComparisons = value; }
        public bool SupportsSubqueriesInExists { get => supportsSubqueriesInExists; set => supportsSubqueriesInExists = value; }
        public bool SupportsSubqueriesInIns { get => supportsSubqueriesInIns; set => supportsSubqueriesInIns = value; }
        public bool SupportsSubqueriesInQuantifieds { get => supportsSubqueriesInQuantifieds; set => supportsSubqueriesInQuantifieds = value; }
        public bool SupportsCorrelatedSubqueries { get => supportsCorrelatedSubqueries; set => supportsCorrelatedSubqueries = value; }
        public bool SupportsUnion { get => supportsUnion; set => supportsUnion = value; }
        public bool SupportsUnionAll { get => supportsUnionAll; set => supportsUnionAll = value; }
        public bool SupportsOpenCursorsAcrossCommit { get => supportsOpenCursorsAcrossCommit; set => supportsOpenCursorsAcrossCommit = value; }
        public bool SupportsOpenCursorsAcrossRollback { get => supportsOpenCursorsAcrossRollback; set => supportsOpenCursorsAcrossRollback = value; }
        public bool SupportsOpenStatementsAcrossCommit { get => supportsOpenStatementsAcrossCommit; set => supportsOpenStatementsAcrossCommit = value; }
        public bool SupportsOpenStatementsAcrossRollback { get => supportsOpenStatementsAcrossRollback; set => supportsOpenStatementsAcrossRollback = value; }
        public int GetMaxBinaryLiteralLength { get => getMaxBinaryLiteralLength; set => getMaxBinaryLiteralLength = value; }
        public int GetMaxCharLiteralLength { get => getMaxCharLiteralLength; set => getMaxCharLiteralLength = value; }
        public int GetMaxColumnNameLength { get => getMaxColumnNameLength; set => getMaxColumnNameLength = value; }
        public int GetMaxColumnsInGroupBy { get => getMaxColumnsInGroupBy; set => getMaxColumnsInGroupBy = value; }
        public int GetMaxColumnsInIndex { get => getMaxColumnsInIndex; set => getMaxColumnsInIndex = value; }
        public int GetMaxColumnsInOrderBy { get => getMaxColumnsInOrderBy; set => getMaxColumnsInOrderBy = value; }
        public int GetMaxColumnsInSelect { get => getMaxColumnsInSelect; set => getMaxColumnsInSelect = value; }
        public int GetMaxColumnsInTable { get => getMaxColumnsInTable; set => getMaxColumnsInTable = value; }
        public int GetMaxConnections { get => getMaxConnections; set => getMaxConnections = value; }
        public int GetMaxCursorNameLength { get => getMaxCursorNameLength; set => getMaxCursorNameLength = value; }
        public int GetMaxIndexLength { get => getMaxIndexLength; set => getMaxIndexLength = value; }
        public int GetMaxSchemaNameLength { get => getMaxSchemaNameLength; set => getMaxSchemaNameLength = value; }
        public int GetMaxProcedureNameLength { get => getMaxProcedureNameLength; set => getMaxProcedureNameLength = value; }
        public int GetMaxCatalogNameLength { get => getMaxCatalogNameLength; set => getMaxCatalogNameLength = value; }
        public int GetMaxRowSize { get => getMaxRowSize; set => getMaxRowSize = value; }
        public bool DoesMaxRowSizeIncludeBlobs { get => doesMaxRowSizeIncludeBlobs; set => doesMaxRowSizeIncludeBlobs = value; }
        public int GetMaxStatementLength { get => getMaxStatementLength; set => getMaxStatementLength = value; }
        public int GetMaxStatements { get => getMaxStatements; set => getMaxStatements = value; }
        public int GetMaxTableNameLength { get => getMaxTableNameLength; set => getMaxTableNameLength = value; }
        public int GetMaxTablesInSelect { get => getMaxTablesInSelect; set => getMaxTablesInSelect = value; }
        public int GetMaxUserNameLength { get => getMaxUserNameLength; set => getMaxUserNameLength = value; }
        public int GetDefaultTransactionIsolation { get => getDefaultTransactionIsolation; set => getDefaultTransactionIsolation = value; }
        public bool SupportsTransactions { get => supportsTransactions; set => supportsTransactions = value; }
        public bool SupportsDataDefinitionAndDataManipulationTransactions { get => supportsDataDefinitionAndDataManipulationTransactions; set => supportsDataDefinitionAndDataManipulationTransactions = value; }
        public bool SupportsDataManipulationTransactionsOnly { get => supportsDataManipulationTransactionsOnly; set => supportsDataManipulationTransactionsOnly = value; }
        public bool DataDefinitionCausesTransactionCommit { get => dataDefinitionCausesTransactionCommit; set => dataDefinitionCausesTransactionCommit = value; }
        public bool DataDefinitionIgnoredInTransactions { get => dataDefinitionIgnoredInTransactions; set => dataDefinitionIgnoredInTransactions = value; }
        public bool SupportsBatchUpdates { get => supportsBatchUpdates; set => supportsBatchUpdates = value; }
        public bool SupportsSavepoints { get => supportsSavepoints; set => supportsSavepoints = value; }
        public bool SupportsNamedParameters { get => supportsNamedParameters; set => supportsNamedParameters = value; }
        public bool SupportsMultipleOpenResults { get => supportsMultipleOpenResults; set => supportsMultipleOpenResults = value; }
        public bool SupportsGetGeneratedKeys { get => supportsGetGeneratedKeys; set => supportsGetGeneratedKeys = value; }
        public int GetDatabaseMajorVersion { get => getDatabaseMajorVersion; set => getDatabaseMajorVersion = value; }
        public int GetDatabaseMinorVersion { get => getDatabaseMinorVersion; set => getDatabaseMinorVersion = value; }
        public int GetJDBCMajorVersion { get => getJDBCMajorVersion; set => getJDBCMajorVersion = value; }
        public int GetJDBCMinorVersion { get => getJDBCMinorVersion; set => getJDBCMinorVersion = value; }
        public int GetSQLStateType { get => getSQLStateType; set => getSQLStateType = value; }
        public bool LocatorsUpdateCopy { get => locatorsUpdateCopy; set => locatorsUpdateCopy = value; }
        public bool SupportsStatementPooling { get => supportsStatementPooling; set => supportsStatementPooling = value; }
        public bool SupportsStoredFunctionsUsingCallSyntax { get => supportsStoredFunctionsUsingCallSyntax; set => supportsStoredFunctionsUsingCallSyntax = value; }
        public bool AutoCommitFailureClosesAllResultSets { get => autoCommitFailureClosesAllResultSets; set => autoCommitFailureClosesAllResultSets = value; }
        public int GetResultSetHoldability { get => getResultSetHoldability; set => getResultSetHoldability = value; }

  
    public override String ToString()
        {
            return "JdbcDatabaseMetaData [getURL=" + getURL + ", isReadOnly=" + isReadOnly
                + ", allProceduresAreCallable=" + allProceduresAreCallable + ", allTablesAreSelectable="
                + allTablesAreSelectable + ", getUserName=" + getUserName + ", nullsAreSortedHigh=" + nullsAreSortedHigh
                + ", nullsAreSortedLow=" + nullsAreSortedLow + ", nullsAreSortedAtStart=" + nullsAreSortedAtStart
                + ", nullsAreSortedAtEnd=" + nullsAreSortedAtEnd + ", getDatabaseProductName=" + getDatabaseProductName
                + ", getDatabaseProductVersion=" + getDatabaseProductVersion + ", getDriverName=" + getDriverName
                + ", getDriverVersion=" + getDriverVersion + ", getDriverMajorVersion=" + getDriverMajorVersion
                + ", getDriverMinorVersion=" + getDriverMinorVersion + ", usesLocalFiles=" + usesLocalFiles
                + ", usesLocalFilePerTable=" + usesLocalFilePerTable + ", supportsMixedCaseIdentifiers="
                + supportsMixedCaseIdentifiers + ", storesUpperCaseIdentifiers=" + storesUpperCaseIdentifiers
                + ", storesLowerCaseIdentifiers=" + storesLowerCaseIdentifiers + ", storesMixedCaseIdentifiers="
                + storesMixedCaseIdentifiers + ", supportsMixedCaseQuotedIdentifiers="
                + supportsMixedCaseQuotedIdentifiers + ", storesUpperCaseQuotedIdentifiers="
                + storesUpperCaseQuotedIdentifiers + ", storesLowerCaseQuotedIdentifiers="
                + storesLowerCaseQuotedIdentifiers + ", storesMixedCaseQuotedIdentifiers="
                + storesMixedCaseQuotedIdentifiers + ", getIdentifierQuoteString=" + getIdentifierQuoteString
                + ", getSQLKeywords=" + getSQLKeywords + ", getNumericFunctions=" + getNumericFunctions
                + ", getStringFunctions=" + getStringFunctions + ", getSystemFunctions=" + getSystemFunctions
                + ", getTimeDateFunctions=" + getTimeDateFunctions + ", getSearchStringEscape=" + getSearchStringEscape
                + ", getExtraNameCharacters=" + getExtraNameCharacters + ", supportsAlterTableWithAddColumn="
                + supportsAlterTableWithAddColumn + ", supportsAlterTableWithDropColumn="
                + supportsAlterTableWithDropColumn + ", supportsColumnAliasing=" + supportsColumnAliasing
                + ", nullPlusNonNullIsNull=" + nullPlusNonNullIsNull + ", supportsConvert=" + supportsConvert
                + ", supportsTableCorrelationNames=" + supportsTableCorrelationNames
                + ", supportsDifferentTableCorrelationNames=" + supportsDifferentTableCorrelationNames
                + ", supportsExpressionsInOrderBy=" + supportsExpressionsInOrderBy + ", supportsOrderByUnrelated="
                + supportsOrderByUnrelated + ", supportsGroupBy=" + supportsGroupBy + ", supportsGroupByUnrelated="
                + supportsGroupByUnrelated + ", supportsGroupByBeyondSelect=" + supportsGroupByBeyondSelect
                + ", supportsLikeEscapeClause=" + supportsLikeEscapeClause + ", supportsMultipleResultSets="
                + supportsMultipleResultSets + ", supportsMultipleTransactions=" + supportsMultipleTransactions
                + ", supportsNonNullableColumns=" + supportsNonNullableColumns + ", supportsMinimumSQLGrammar="
                + supportsMinimumSQLGrammar + ", supportsCoreSQLGrammar=" + supportsCoreSQLGrammar
                + ", supportsExtendedSQLGrammar=" + supportsExtendedSQLGrammar + ", supportsANSI92EntryLevelSQL="
                + supportsANSI92EntryLevelSQL + ", supportsANSI92IntermediateSQL=" + supportsANSI92IntermediateSQL
                + ", supportsANSI92FullSQL=" + supportsANSI92FullSQL + ", supportsIntegrityEnhancementFacility="
                + supportsIntegrityEnhancementFacility + ", supportsOuterJoins=" + supportsOuterJoins
                + ", supportsFullOuterJoins=" + supportsFullOuterJoins + ", supportsLimitedOuterJoins="
                + supportsLimitedOuterJoins + ", getSchemaTerm=" + getSchemaTerm + ", getProcedureTerm="
                + getProcedureTerm + ", getCatalogTerm=" + getCatalogTerm + ", isCatalogAtStart=" + isCatalogAtStart
                + ", getCatalogSeparator=" + getCatalogSeparator + ", supportsSchemasInDataManipulation="
                + supportsSchemasInDataManipulation + ", supportsSchemasInProcedureCalls="
                + supportsSchemasInProcedureCalls + ", supportsSchemasInTableDefinitions="
                + supportsSchemasInTableDefinitions + ", supportsSchemasInIndexDefinitions="
                + supportsSchemasInIndexDefinitions + ", supportsSchemasInPrivilegeDefinitions="
                + supportsSchemasInPrivilegeDefinitions + ", supportsCatalogsInDataManipulation="
                + supportsCatalogsInDataManipulation + ", supportsCatalogsInProcedureCalls="
                + supportsCatalogsInProcedureCalls + ", supportsCatalogsInTableDefinitions="
                + supportsCatalogsInTableDefinitions + ", supportsCatalogsInIndexDefinitions="
                + supportsCatalogsInIndexDefinitions + ", supportsCatalogsInPrivilegeDefinitions="
                + supportsCatalogsInPrivilegeDefinitions + ", supportsPositionedDelete=" + supportsPositionedDelete
                + ", supportsPositionedUpdate=" + supportsPositionedUpdate + ", supportsSelectForUpdate="
                + supportsSelectForUpdate + ", supportsStoredProcedures=" + supportsStoredProcedures
                + ", supportsSubqueriesInComparisons=" + supportsSubqueriesInComparisons
                + ", supportsSubqueriesInExists=" + supportsSubqueriesInExists + ", supportsSubqueriesInIns="
                + supportsSubqueriesInIns + ", supportsSubqueriesInQuantifieds=" + supportsSubqueriesInQuantifieds
                + ", supportsCorrelatedSubqueries=" + supportsCorrelatedSubqueries + ", supportsUnion=" + supportsUnion
                + ", supportsUnionAll=" + supportsUnionAll + ", supportsOpenCursorsAcrossCommit="
                + supportsOpenCursorsAcrossCommit + ", supportsOpenCursorsAcrossRollback="
                + supportsOpenCursorsAcrossRollback + ", supportsOpenStatementsAcrossCommit="
                + supportsOpenStatementsAcrossCommit + ", supportsOpenStatementsAcrossRollback="
                + supportsOpenStatementsAcrossRollback + ", getMaxBinaryLiteralLength=" + getMaxBinaryLiteralLength
                + ", getMaxCharLiteralLength=" + getMaxCharLiteralLength + ", getMaxColumnNameLength="
                + getMaxColumnNameLength + ", getMaxColumnsInGroupBy=" + getMaxColumnsInGroupBy
                + ", getMaxColumnsInIndex=" + getMaxColumnsInIndex + ", getMaxColumnsInOrderBy="
                + getMaxColumnsInOrderBy + ", getMaxColumnsInSelect=" + getMaxColumnsInSelect
                + ", getMaxColumnsInTable=" + getMaxColumnsInTable + ", getMaxConnections=" + getMaxConnections
                + ", getMaxCursorNameLength=" + getMaxCursorNameLength + ", getMaxIndexLength=" + getMaxIndexLength
                + ", getMaxSchemaNameLength=" + getMaxSchemaNameLength + ", getMaxProcedureNameLength="
                + getMaxProcedureNameLength + ", getMaxCatalogNameLength=" + getMaxCatalogNameLength
                + ", getMaxRowSize=" + getMaxRowSize + ", doesMaxRowSizeIncludeBlobs=" + doesMaxRowSizeIncludeBlobs
                + ", getMaxStatementLength=" + getMaxStatementLength + ", getMaxStatements=" + getMaxStatements
                + ", getMaxTableNameLength=" + getMaxTableNameLength + ", getMaxTablesInSelect=" + getMaxTablesInSelect
                + ", getMaxUserNameLength=" + getMaxUserNameLength + ", getDefaultTransactionIsolation="
                + getDefaultTransactionIsolation + ", supportsTransactions=" + supportsTransactions
                + ", supportsDataDefinitionAndDataManipulationTransactions="
                + supportsDataDefinitionAndDataManipulationTransactions + ", supportsDataManipulationTransactionsOnly="
                + supportsDataManipulationTransactionsOnly + ", dataDefinitionCausesTransactionCommit="
                + dataDefinitionCausesTransactionCommit + ", dataDefinitionIgnoredInTransactions="
                + dataDefinitionIgnoredInTransactions + ", supportsBatchUpdates=" + supportsBatchUpdates
                + ", supportsSavepoints=" + supportsSavepoints + ", supportsNamedParameters=" + supportsNamedParameters
                + ", supportsMultipleOpenResults=" + supportsMultipleOpenResults + ", supportsGetGeneratedKeys="
                + supportsGetGeneratedKeys + ", getDatabaseMajorVersion=" + getDatabaseMajorVersion
                + ", getDatabaseMinorVersion=" + getDatabaseMinorVersion + ", getJDBCMajorVersion="
                + getJDBCMajorVersion + ", getJDBCMinorVersion=" + getJDBCMinorVersion + ", getSQLStateType="
                + getSQLStateType + ", locatorsUpdateCopy=" + locatorsUpdateCopy + ", supportsStatementPooling="
                + supportsStatementPooling + ", supportsStoredFunctionsUsingCallSyntax="
                + supportsStoredFunctionsUsingCallSyntax + ", autoCommitFailureClosesAllResultSets="
                + autoCommitFailureClosesAllResultSets + ", getResultSetHoldability=" + getResultSetHoldability + "]";
        }

    }
}
