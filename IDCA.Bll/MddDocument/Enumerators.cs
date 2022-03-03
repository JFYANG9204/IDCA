
namespace IDCA.Bll.MddDocument
{

    public enum Alignments
    {
        alCenter,
        alDefault,
        alJustify,
        alLeft,
        alRight,
    }

    public enum VerticalAlignments
    {
        vaBaseline,
        vaBottom,
        vaDefault,
        vaMiddle,
        vaSub,
        vaSuper,
        vaTextBottom,
        vaTextTop,
        vaTop
    }

    public enum ArrayTypeConstants
    {
        mlExpand,
        mlLevel
    }

    public enum AskTypeConstants
    {
        atArray,
        atDefault,
        atGrid
    }

    public enum AudioControlPositions
    {
        cpBottom,
        cpLeft,
        cpRight,
        cpTop,
    }

    public enum BorderStyles
    {
        bsDouble,
        bsGroove,
        bsInset,
        bsNone,
        bsOutset,
        bsRidge,
        bsSolid
    }

    public enum CategoryFilterConstants
    {
        cfAll,
        cfCategory,
        cfElement,
        cfExpression,
        cfOther
    }

    public enum CategoryFlagConstants
    {
        flDontKnow,
        flExclusive,
        flFixedPosition,
        flInline,
        flMultiplier,
        flNoanswer,
        flNoFilter,
        flNone,
        flOther,
        flRefuse,
        flUser
    }

    public enum ContextUsageConstants
    {
        cuLabels,
        cuProperties,
        cuRoutings
    }

    public enum ControlTypes
    {
        ctButton,
        ctCheckButton,
        ctComboList,
        ctDate,
        ctDateTime,
        ctDropList,
        ctEdit,
        ctListBox,
        ctListControl,
        ctMultiLineEdit,
        ctPassword,
        ctRadioButton,
        ctSingleLineEdit,
        ctStatic,
        ctTime
    }

    public enum CursorTypes
    {
        crAuto,
        crCrossHair,
        crDefault,
        crEResize,
        crHelp,
        crMove,
        crNEResize,
        crNResize,
        crNWResize,
        crPointer,
        crSEResize,
        crSResize,
        crSWResize,
        crText,
        crWait,
        crWResize,
    }

    public enum DataTypeConstants
    {
        mtBoolean,
        mtCategorical,
        mtData,
        mtDouble,
        mtLong,
        mtNone,
        mtObject,
        mtText
    }

    public enum DBQuestionsTypesConstants
    {
        dtArray,
        dtMultipleResponse,
        dtSingleResponse,
        dtUnknown
    }

    public enum ElementAlignments
    {
        eaDefault,
        eaNewLine,
        eaRight
    }

    public enum ElementTypeConstants
    {
        mtAnalysisBase,
        mtAnalysisCategory,
        mtAnalysisMaximum,
        mtAnalysisMean,
        mtAnalysisMinimun,
        mtAnalysiSampleVariance,
        mtAnalysisStdDev,
        mtAnalysisStdErr,
        mtAnalysisSubheading,
        mtAnalysisSubtotal,
        mtAnalysisSummaryData,
        mtAnalysisTotal,
        mtCategory
    }

    public enum ExObjectTypeConstants
    {
        etAliasMap,
        etArray,
        etCategories,
        etCategoryMap,
        etClass,
        etCompound,
        etContext,
        etContextAlternatives,
        etContexts,
        etDataSource,
        etDataSources,
        etDocument,
        etElement,
        etElementInstance,
        etElementInstances,
        etElementList,
        etElements,
        etField,
        etFields,
        etFolder,
        etGoto,
        etGrid,
        etGridMulti,
        etGridNumeric,
        etGridSingle,
        etGridText,
        etHelperFields,
        etIndexElement,
        etIndicesElements,
        etItems,
        etLabel,
        etLanguage,
        etLanguages,
        etLevelObject,
        etNote,
        etNotes,
        etPage,
        etPages,
        etParameters,
        etProperties,
        etRanges,
        etRouting,
        etRoutingItem,
        etRoutingItems,
        etStyle,
        etTemplate,
        etTemplates,
        etTypes,
        etUnknown,
        etVariable,
        etVariableBoolean,
        etVariableDate,
        etVariableDouble,
        etVariableInfo,
        etVariableInstance,
        etVariableLong,
        etVariableMulti,
        etVariableObject,
        etVariables,
        etVariableSingle,
        etVariableText,
        etVersion,
        etVersions
    }

    public enum FilterTypeConstants
    {
        ftExclude,
        ftInclude
    }

    public enum ImagePositions
    {
        ipBottom,
        ipImageOnly,
        ipLeft,
        ipNone,
        ipRight,
        ipTop,
    }

    public enum InfoTypeConstants
    {
        mtFlags,
        mtMissing,
        mtOther,
        mtValues
    }

    public enum InterviewModesConstants
    {
        imEmpty,
        imLocal,
        imPaper_Manual_Entry,
        imPaper_Scanning,
        imPhone,
        imWeb
    }

    public enum IteratorTypeConstants
    {
        itCategorical,
        itNumericRanges
    }

    public enum JoinOptionConstants
    {
        joDataSourcesAppendOnly,
        joExcludeBaseLanguage,
        joExcludeDataSources,
        joExcludeLabels,
        joExcludeProperties,
        joExcludeRouting,
        joExcludeSystemVariables,
        joFullDataSources,
        joFullLCL,
        joFullUnversioned,
        joIgnoreConflicts,
        joLeftAlternatives,
        joNamespace,
        joRenameConflicts,
        joRightLabels,
        joRightQuestions,
        joSkipUndoOnFail,
        joUpdateSecondDoc
    }

    public enum JoinTypeConstants
    {
        jtFull,
        jtLeft,
        jtRight,
    }

    public enum LanguageConstants
    {
        langICOUNTRY,
        langIDEFAULTANSICODEPAGE,
        langIDEFAULTCODEPAGE,
        langIDEFAULTCOUNTRY,
        langIDEFAULTLANGUAGE,
        langILANGUAGE,
        langSABBREVCTRYNAME,
        langSABBREVLANGNAME,
        langSCOUNTRY,
        langSENGCOUNTRY,
        langSENGLANGUAGE,
        langSISO3166CTRYNAME,
        langSISO639LANGNAME,
        langSLANGUAGE,
        langSNATIVECTRYNAME,
        langSNATIVELANGNAME
    }

    public enum ModifierType
    {
        ModifierSigBool,
        ModifierSigDouble,
        ModifierSigLong,
        ModifierSigObject,
        ModifierSigString,
        ModifierSigVariant
    }

    public enum ObjectTypesConstants
    {
        mtAliasMap,
        mtArray,
        mtCategories,
        mtCategoryMap,
        mtClass,
        mtCompound,
        mtConditionalRouting,
        mtContext,
        mtContextAlternatives,
        mtContexts,
        mtDataSource,
        mtDataSources,
        mtDBElements,
        mtDBQuestionDataProvider,
        mtDocument,
        mtElement,
        mtElementInstance,
        mtElementInstances,
        mtElementList,
        mtElements,
        mtField,
        mtFields,
        mtGoto,
        mtGrid,
        mtHelperFields,
        mtIfBlock,
        mtIndexElement,
        mtIndicesElements,
        mtItems,
        mtLabel,
        mtLanguage,
        mtLanguages,
        mtLevelObject,
        mtNote,
        mtNotes,
        mtPage,
        mtPages,
        mtParameters,
        mtProperties,
        mtRanges,
        mtRouting,
        mtRoutingItem,
        mtRoutingItems,
        mtStyle,
        mtTemplate,
        mtTemplates,
        mtTypes,
        mtUnknown,
        mtVariable,
        mtVariableInstance,
        mtVariables,
        mtVersion,
        mtVersions
    }

    public enum OpenConstants
    {
        oNOSAVE,
        oREAD,
        oREADWRITE,
    }

    public enum OrderConstants
    {
        oAscending,
        oCustom,
        oDescending,
        oNormal,
        oRandomize,
        oReverse,
        oRotate
    }

    public enum OrientationConstants
    {
        oCol,
        oRow,
    }

    public enum Orientations
    {
        orColumn,
        orDefault,
        orRow
    }

    public enum RecordModes
    {
        rmAuto,
        rmManual,
        rmNone,
        rmProhibited
    }

    public enum RoutingItemsFlags
    {
        riArray,
        riEmpty,
        riExcludeHidden,
        riExpandedArray,
        riExpandedClass,
        riExpandedCompound,
        riExpandedGrid,
        riExpandedPage
    }

    public enum RoutingScriptOptions
    {
        rsDefault,
        rsExitWithItem
    }

    public enum RoutingTypeConstants
    {
        rArray,
        rClass,
        rComment,
        rCompound,
        rConditionalGoto,
        rEmpty,
        rExit,
        rGoto,
        rGrid,
        rIfBlock,
        rObject,
        rPage,
        rQCItem,
        rScriptBasicItem,
        rScriptLabel,
        rSetResponse,
        rVariable
    }

    public enum SourceTypeConstants
    {
        sDataField,
        sExpression,
        sExpressions,
        sNoCaseData,
        sNone
    }

    public enum VariableUsageConstants
    {
        vtArray,
        vtClass,
        vtCoding,
        vtCompound,
        vtFilter,
        vtGrid,
        vtHelperField,
        vtMultiplier,
        vtOtherSpecify,
        vtSourceFile,
        vtVariable,
        vtWeight
    }

}
