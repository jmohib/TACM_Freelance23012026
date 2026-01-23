namespace TACM.Core
{
    public static class AppConstants
    {
        public const string DB_CONNECTION_STRING_SETTINGS_NAME = "Database";
        public const string SHARED_SETTINGS_FILENAME = "sharedsettings.json";

        public const ushort DEFAULT_VERBAL_MEMORY_WORDS_QUANTITY = 25;
        public const ushort DEFAULT_VERBAL_MEMORY_DEMO_WORDS_QUANTITY = 3;

        public const ushort DEFAULT_NONVERBAL_MEMORY_WORDS_QUANTITY = 25;
        public const ushort DEFAULT_NONVERBAL_MEMORY_DEMO_WORDS_QUANTITY = 3;

        public const ushort DEFAULT_WORD_TEST_FONTSIZE = 72;
        public const ushort AWAIT_TIME_TO_MEMORIZE_OBJECT = 3000;

        public const ushort SECONDS_TO_STAY_WORDS = 5;
        public const ushort SECONDS_BETWEEN_WORDS = 5;
        public const ushort RANDOM_DRAW_OBJECTS_COUNT_DEMO = 3;
        public const ushort RANDOM_DRAW_OBJECTS_COUNT_FOR_REAL = 25;

        public const string CORRECT_ANSWER_BG_COLOR = "#7ff081";
        public const string WRONG_ANSWER_BG_COLOR = "#eda79f";
        public const string DEFAULT_ANSWER_BG_COLOR = "#C8C8C8";

        public const string OBJECT_TYPE_WORDS_ON_PLURAL = "words";
        public const string OBJECT_TYPE_PICTURES_ON_PLURAL = "pictures";
        public const string OBJECT_TYPE_ATTENTION_TEST = "attention";

        public const string OBJECT_TYPE_WORDS_ON_SINGULAR = "word";
        public const string OBJECT_TYPE_PICTURES_ON_SINGULAR = "picture";

        public const string VERBAL_MEMORY_TEST_TYPE = "verbal-memory";
        public const string NON_VERBAL_MEMORY_TEST_TYPE = "non-verbal-memory";
        public const string ATTENTION_TEST_TYPE = "attention";

        public static char[] ALPHABET_AND_NUMBERS = [
            'A', 'B', 'C', 'D', 'E', 'F',
            'G', 'H', 'I', 'J', 'K', 'L', 
            'M', 'N', 'O', 'P', 'Q', 'R', 
            'S', 'T', 'U', 'V', 'W', 'X', 
            'Y', 'Z', '0', '1', '2', '3',
            '4', '5', '6', '7', '8', '9'
        ];

        public static Type PAGE_KEY_EVENT_HANDLER_INTERFACE_TYPE = typeof(IPageKeyEventHandler);

        public const ushort DEFAULT_T1 = 2500;
        public const ushort DEFAULT_T2 = 1500;
        public const ushort DEFAULT_T3 = 3500;
        public const ushort DEFAULT_T4 = 3500;

        public static IReadOnlyDictionary<string, string> SEX_DICTIONARIES = new Dictionary<string, string>
        {
            { "F", "Female" },
            { "M", "Male" },
            { "Female", "F" },
            { "Male", "M" }
        };
    }
}
