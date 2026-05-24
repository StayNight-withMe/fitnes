using fitnes.Domain.Enums;

namespace fitnes.Domain.Constants.Localization;

public static class LocalizationKeysConstants
{
    public static class Idle
    {
        public const string Text = "Text";
        public const string BtnStart = "BtnStart";
        public const string BtnLanguage = "BtnLanguage";
    }

    public static class LanguageSelection
    {
        public const string Text = "Text";
        public const string Current = "Current";
        public static readonly (string LangName, Language LangValue)[] AvailableLanguages = { ("BtnRu", Language.Ru), ("BtnEn", Language.En), ("BtnEs", Language.Es), ("BtnDe", Language.De), ("BtnFr", Language.Fr) };
    }

    public static class WorkMenu
    {
        public const string Text = "Text";
        public const string BtnCalories = "BtnCalories";
        public const string BtnProfile = "BtnProfile";
        public const string BtnGoals = "BtnGoals";
        public const string BtnNutrition = "BtnNutrition";
        public const string BtnBack = "BtnBack";
        public const string BtnCancel = "BtnCancel";
    }

    public static class AnalysisResult
    {
        public const string Dish = "Dish";
        public const string Accuracy = "Accuracy";
        public const string Serving = "Serving";
        public const string Calories = "Calories";
        public const string Protein = "Protein";
        public const string Fat = "Fat";
        public const string Carbs = "Carbs";
        public const string UnknownDish = "UnknownDish";
        public const string NoAnalysis = "NoAnalysis";
        public const string DefaultUnit = "DefaultUnit";
        public const string DefaultServingDescription = "DefaultServingDescription";
        public const string BtnSave = "BtnSave";
        public const string Saved = "Saved";
        public const string AlreadySaved = "AlreadySaved";
    }

    public static class Calories
    {
        public const string Text = "Text";
    }

    public static class Profile
    {
        public const string AskWeight = "AskWeight";
        public const string AskHeight = "AskHeight";
        public const string AskAge = "AskAge";
        public const string AskGender = "AskGender";
        public const string InvalidGender = "InvalidGender";
        public const string AskWaist = "AskWaist";
        public const string AskNeck = "AskNeck";
        public const string AskHips = "AskHips";
        public const string InvalidNumber = "InvalidNumber";
        public const string AskTimezone = "AskTimezone";
        public const string InvalidTimezone = "InvalidTimezone";
        public const string Saved = "Saved";
        public const string ProfileSaved = "ProfileSaved";
        public const string Card = "Card";
        public const string BtnEdit = "BtnEdit";
        public const string NotSet = "NotSet";
    }

    public static class Errors
    {
        public const string SessionExpired = "SessionExpired";
    }

    public static class Stats
    {
        public const string Card = "Card";
        public const string Empty = "Empty";
        public const string BtnDay = "BtnDay";
        public const string BtnWeek = "BtnWeek";
        public const string BtnMonth = "BtnMonth";
        public const string BtnToday = "BtnToday";
        public const string BtnPrev = "BtnPrev";
        public const string BtnNext = "BtnNext";
        public const string Current = "Current";
    }

    public static class Goals
    {
        public const string MenuText = "MenuText";
        public const string BtnNew = "BtnNew";
        public const string BtnView = "BtnView";
        public const string TypeText = "TypeText";
        public const string BtnLose = "BtnLose";
        public const string BtnGain = "BtnGain";
        public const string BtnMaintain = "BtnMaintain";
        public const string AskWeight = "AskWeight";
        public const string AskActivity = "AskActivity";
        public const string ActivitySedentary = "ActivitySedentary";
        public const string ActivitySedentaryDesc = "ActivitySedentaryDesc";
        public const string ActivityLight = "ActivityLight";
        public const string ActivityLightDesc = "ActivityLightDesc";
        public const string ActivityModerate = "ActivityModerate";
        public const string ActivityModerateDesc = "ActivityModerateDesc";
        public const string ActivityActive = "ActivityActive";
        public const string ActivityActiveDesc = "ActivityActiveDesc";
        public const string ActivityVeryActive = "ActivityVeryActive";
        public const string ActivityVeryActiveDesc = "ActivityVeryActiveDesc";
        public const string Card = "Card";
        public const string Empty = "Empty";
        public const string NeedWeight = "NeedWeight";
    }
}
