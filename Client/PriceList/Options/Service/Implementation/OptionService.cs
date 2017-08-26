using DatabaseService.DataService;

namespace Options.Service.Implementation
{
    public class OptionService : IOptionService
    {
        #region Members

        private readonly IDataService dataService;
        private string login;
        private string password;
        private long lastOrderNumber;

        #endregion

        #region Constructors

        public OptionService(IDataService dataService)
        {
            this.dataService = dataService;
            InitProperties();
        }

        #endregion

        #region Properties

        public string Login
        {
            get
            {
                return login;
            }
            set
            {
                if (login != value)
                {
                    login = value;
                    SetOption(OptionName.Login, value);
                }
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (password != value)
                {
                    password = value;
                    SetOption(OptionName.Password, value);
                }
            }
        }

        public long LastOrderNumber
        {
            get
            {
                return lastOrderNumber;
            }
            set
            {
                if (lastOrderNumber != value)
                {
                    lastOrderNumber = value;
                    SetOption(OptionName.LastOrderNumber, value.ToString());
                }
            }
        }

        #endregion

        #region Methods

        private void InitProperties()
        {
            login = GetOption(OptionName.Login);
            password = GetOption(OptionName.Password);
            lastOrderNumber = long.TryParse(GetOption(OptionName.LastOrderNumber), out lastOrderNumber)
                ? lastOrderNumber
                : 0;
        }

        private string GetOption(string optionCode)
        {
            string value = dataService.GetOption(optionCode);

            return value;
        }

        private void SetOption(string optionCode, string value)
        {
            dataService.SetOption(optionCode, value);
        }

        #endregion
    }
}
