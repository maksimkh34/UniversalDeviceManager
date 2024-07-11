namespace UDM.Model.SettingsService
{
    public delegate bool ValidateSettingValue(object value);              // Делегат для проверки, допустимо ли значение

    public delegate dynamic CorrectSettingValue(dynamic value);           // Откорректировать значение если оно допустимо

    public delegate void SettingChanged(SettingChangedContext context);   // Замена INotifyPropertyChanged

    public class Setting(string settingName, dynamic? defaultValue, Type settingType,
        ValidateSettingValue? validateSettingValue,
        CorrectSettingValue? correctSettingValue,
        SettingChanged? settingChanged,
        string displayName,
        bool isUsingPossibleValues, IEnumerable<string>? possibleValues)
    {
        // Стандартное значение
        private readonly Type? _settingType = settingType;  // Тип переменной

        // Если истина, настройка может содержать только значения из _possibleValues
        public string DisplayName { get; set; } = displayName;       // Как переменная отображается для пользователя

        private readonly ValidateSettingValue _localValidateSettingValue = validateSettingValue ?? DefaultValidateSettingValue;
        private readonly CorrectSettingValue _localCorrectSettingValue = correctSettingValue ?? DefalutCorrectSettingValue;
        private SettingChanged _localSettingChanged = settingChanged ?? DefaultSettingChanged;

        public void UpdateValueChanged(SettingChanged foo)
        {
            _localSettingChanged = foo;
        }

        public string Name { get; set; } = settingName;

        public dynamic? Value
        {
            get => Convert.ChangeType(defaultValue, _settingType ?? typeof(string)); // Конвертация значения в тип настройки
            set
            {
                if (value?.GetType() == _settingType && _localValidateSettingValue(value)) // Проверка, совпадает ли тип переданного значения и настройки
                {
                    if (!isUsingPossibleValues || isUsingPossibleValues && IsValuePossible(value)) // Проверка на присутствие значения в _possibleValues, если нужно
                    {
                        var oldValue = defaultValue;
                        defaultValue = _localCorrectSettingValue(value);
                        _localSettingChanged(new SettingChangedContext(oldValue, value));
                    }
                    else throw new SettingsExceptions.ValueIsNotAllowed();
                }
                else throw new SettingsExceptions.InvalidSettingValueType("Invalid Setting Value Type");
            }
        }

        public bool IsUsingPossibleValues() => isUsingPossibleValues;

        public IEnumerable<string>? GetPossibleValues() => possibleValues;

        // Стандартные значения делегатов

        private static bool DefaultValidateSettingValue(object value) => true;

        private static dynamic DefalutCorrectSettingValue(dynamic value) => value;

        private static void DefaultSettingChanged(SettingChangedContext context)
        { }

        private bool IsValuePossible(string value)  // Есть ли значение в массиве разрешенных
        {
            return (possibleValues ?? new List<string>()).Any(pValue => value == pValue);
        }

        public Type GetSettingType() => _settingType ?? typeof(string);
    }

    public class SettingsStorage(string localFilePath)    // Система настроек программы, позволяющая абстрагировать пользователя от класса настройки
    {
        private readonly List<Setting> _settingsStorage = new();

        public object? GetValue(string settingName)   // Возвращает значение настройки по ее имени
        {
            foreach (var setting in _settingsStorage.Where(setting => setting.Name == settingName))
            {
                return setting.Value;
            }

            throw new SettingsExceptions.SettingNotExists();
        }

        public Setting Get(string settingName)   // Возвращает значение настройки по ее имени
        {
            foreach (var setting in _settingsStorage.Where(setting => setting.Name == settingName))
            {
                return setting;
            }

            throw new SettingsExceptions.SettingNotExists();
        }

        public List<string> GetSettingsNames()
        {
            return _settingsStorage.Select(setting => setting.Name).ToList();
        }

        public void Set(string settingName, object settingValue)  // Установить настройку
        {
            foreach (var setting in _settingsStorage.Where(setting => setting.Name == settingName))
            {
                setting.Value = settingValue;
            }
        }

        public void SaveSettings()    // Сохранить настройки в файл
        {
            Dictionary<string, string> result = new();
            foreach (var setting in _settingsStorage)
            {
                if (setting.Value is null) throw new NullReferenceException("Setting value was null. ");
                result.Add(setting.Name, Writable.GetWritableValue(setting.Value.ToString()));
            }
            DataProvider.WriteDataDict(localFilePath, result);
        }

        public void LoadSettings()
        {
            var unparsedSettings = DataProvider.LoadDataDict(localFilePath);
            foreach (var setting in unparsedSettings)
            {
                Set(setting.Key, Writable.GetOriginalFromWritable(setting.Value));
            }
        }

        public void Register(Setting setting)
        {
            _settingsStorage.Add(setting);
        }

        public List<string[]> GetSettingsValues()
        {
            return _settingsStorage.Select(setting => new string[] { setting.Name, setting.DisplayName, setting.GetSettingType().ToString(), setting.Value ?? throw new NullReferenceException("Setting was null") }).ToList();
        }
    }

    public class SettingsExceptions
    {
        public class ValueIsNotAllowed : Exception
        {
            public ValueIsNotAllowed()
            {
            }

            public ValueIsNotAllowed(string message)
                : base(message)
            {
            }

            public ValueIsNotAllowed(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class InvalidSettingValueType : Exception
        {
            public InvalidSettingValueType()
            {
            }

            public InvalidSettingValueType(string message)
                : base(message)
            {
            }

            public InvalidSettingValueType(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class SettingNotExists : Exception
        {
            public SettingNotExists()
            {
            }

            public SettingNotExists(string message)
                : base(message)
            {
            }

            public SettingNotExists(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class InvalidSettingValue : Exception
        {
            public InvalidSettingValue()
            {
            }

            public InvalidSettingValue(string message)
                : base(message)
            {
            }

            public InvalidSettingValue(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }

    public class SettingChangedContext(object oldValue, object newValue)
    {
        public object OldValue = oldValue;
        public object NewValue = newValue;
    }

    public interface ISettingValueCallbacks
    {
        bool ValidateSettingValue(object value);

        dynamic CorrectSettingValue(dynamic value);

        public void SettingChanged(SettingChangedContext context);
    }
}