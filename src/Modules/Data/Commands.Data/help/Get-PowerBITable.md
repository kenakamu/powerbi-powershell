---
external help file: Microsoft.PowerBI.Commands.Data.dll-Help.xml
Module Name: MicrosoftPowerBIMgmt.Data
online version:
schema: 2.0.0
---

# Get-PowerBITable

## SYNOPSIS
Returns a list of Power BI tables.

## SYNTAX

### DatasetId (Default)
```
Get-PowerBITable -DatasetId <Guid> [-Name <String>] [-First <Int32>] [-Skip <Int32>] [-WorkspaceId <Guid>]
 [-Workspace <Workspace>] [<CommonParameters>]
```

### Dataset
```
Get-PowerBITable -Dataset <Dataset> [-Name <String>] [-First <Int32>] [-Skip <Int32>] [-WorkspaceId <Guid>]
 [-Workspace <Workspace>] [<CommonParameters>]
```

## DESCRIPTION
Retrieves a list of Power BI tables in the dataset that match the specified search criteria.
Before you run this command, make sure you log in using Login-PowerBIServiceAccount. 

## EXAMPLES

### Example 1
```powershell
PS C:\> Get-PowerBITable -DatasetId eed49d27-8e3c-424d-9342-c6b3ca6db64d
```

Returns a list of all Power BI tables in dataset eed49d27-8e3c-424d-9342-c6b3ca6db64d

### Example 2
```powershell
PS C:\> Get-PowerBIDataset | ? AddRowsApiEnabled -eq $true | Get-PowerBITable
```

Returns a list of all Power BI tables in datasets which supports AddRowApi.

## PARAMETERS

### -Dataset
A dataset where tables stored. You can pass it via pipe.

```yaml
Type: Dataset
Parameter Sets: Dataset
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -DatasetId
An id of dataset where tables stored.

```yaml
Type: Guid
Parameter Sets: DatasetId
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -First
First (top) list of results.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases: Top

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Name of the table to return if one exists with that name. Case insensitive search.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Skip
Skips the first set of results.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Workspace
Workspace to filter results to, tables only belonging to that workspace are shown.

```yaml
Type: Workspace
Parameter Sets: (All)
Aliases: Group

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WorkspaceId
Workspace ID to filter results to, tables only belonging to that workspace are shown.

```yaml
Type: Guid
Parameter Sets: (All)
Aliases: GroupId

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Microsoft.PowerBI.Common.Api.Datasets.Dataset

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS