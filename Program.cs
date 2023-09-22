using System;
using System.IO;
using System.Text.RegularExpressions;
using Serilog;

class Program
{
	static void Main()
	{
		Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose()
			.WriteTo.Console().WriteTo.File("log.txt").CreateLogger();

		Log.Information("Запуск программы");
		

		RegistrationAcc();

		Log.Information("Завершение программы");
		Log.CloseAndFlush();
	}
	public static void RegistrationAcc()
	{
		Console.WriteLine("РЕГИСТРАЦИЯ");
		Console.WriteLine("Укажите логин, номер телефона (должен начинаться с +7) или электронную почту.\nУкажите пароль.\nПодтверждение пароля.");
		string login = Console.ReadLine();
		string password = Console.ReadLine();
		string confPassword = Console.ReadLine();

		if (login.Contains("+7"))
		{
			RegisterWithPhoneNumber(login, password, confPassword);


		}
		else if (login.Contains("@"))
		{
			RegisterWithEmail(login, password, confPassword);


		}
		else
		{
			RegisterWithLogin(login, password, confPassword);


		}
	}

	private static bool CheckLoginExists(string login)
	{
		string filePath = "D:\\Visual Studio\\C#\\Lab1Registr\\Lab1Registr\\Logins.txt";


		string[] logins = ReadFileContents(filePath);


		foreach (string storedLogin in logins)
		{
			if (storedLogin.Trim().Equals(login.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				return true; // Логин найден в файле
			}
		}

		return false; // Логин не найден в файле
	}

	private static string[] ReadFileContents(string filePath)
	{
		try
		{
			return File.ReadAllLines(filePath);
		}
		catch (FileNotFoundException)
		{
			
			Log.Information("Файл не найден.");
			return new string[0];
		}
		catch (Exception ex)
		{
			
			Log.Information("Ошибка чтения файла.");
			return new string[0];
		}
	}
	private static void WriteToFile(string registrationInfo)
	{
		string filePath = "D:\\Visual Studio\\C#\\Lab1Registr\\Lab1Registr\\Logins.txt";

		try
		{
			using (StreamWriter writer = new StreamWriter(filePath, true))
			{
				writer.WriteLine(registrationInfo);
				
				Log.Information("Запись успешно сохранена в файле.");
			}
		}
		catch (Exception ex)
		{
			
			Log.Information("Ошибка при записи в файл.");
		}
	}
	private static void PasswordTest(string login,string password, string passwordTwo)
	{
		bool isPasswordMatch = CheckPasswordMatch(password, passwordTwo);
		if (!isPasswordMatch)
		{
			Console.WriteLine("False");
			Log.Information("Пароли не совпадают: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
			
		}
		

		bool isValidLength = ValidatePasswordLength(password);
		if (!isValidLength)
		{
			Console.WriteLine("False");
			Log.Information("Длина пароля должна быть не менее 7 символов: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		bool isValidCyrillic = ValidateCyrillic(password);
		if (!isValidCyrillic)
		{
			Console.WriteLine("False");
			Log.Information("Пароль должен содержать только символы кириллицы: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		bool hasUppercase = ContainsUppercase(password);
		bool hasLowercase = ContainsLowercase(password);

		if (!hasUppercase || !hasLowercase)
		{
			Console.WriteLine("False");
			Log.Information("Пароль должен содержать символы верхнего и нижнего регистра: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		bool hasDigits = ContainsDigits(password);
		if (!hasDigits)
		{
			Console.WriteLine("False");
			Log.Information("Пароль должен содержать цифры: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		bool hasSpecialCharacters = ContainsSpecialCharacters(password);
		if (!hasSpecialCharacters)
		{
			Console.WriteLine("False");
			Log.Information("Пароль должен содержать спецсимволы: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		Console.WriteLine("True");
		Log.Information("Успешная регистрация: Дата-время запроса: {DateTime}," +
			" Логин: {Login}," +
			" Пароль: {MaskedPassword}," +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
		
		string registrationInfo = $"{login}";
		WriteToFile(registrationInfo);
	}

	private static bool ContainsUppercase(string password)
	{
		return password.Any(char.IsUpper);
	}

	private static bool ContainsLowercase(string password)
	{
		return password.Any(char.IsLower);
	}
	
	private static bool ValidatePasswordLength(string password)
	{
		return password.Length >= 7;
	}
	private static bool CheckPasswordMatch(string password, string confPassword)
	{
		return password == confPassword;
	}
	private static bool ContainsDigits(string password)
	{
		return password.Any(char.IsDigit);
	}
	private static bool ContainsSpecialCharacters(string password)
	{
		string specialCharactersPattern = @"[!@#$%^&*(),.?""':{}\[\]|\\\/~`_+=;<>]";
		return Regex.IsMatch(password, specialCharactersPattern);
	}

	static string MaskPassword(string password)
	{
		char[] maskedChars = new char[password.Length];

		for (int i = 0; i < password.Length; i++)
		{
			char currentChar = password[i];
			char maskedChar = GetNextChar(currentChar);
			maskedChars[i] = maskedChar;
		}

		return new string(maskedChars);
	}

	static char GetNextChar(char currentChar)
	{
		if (currentChar == 'z')
			return 'a';
		if (currentChar == 'Z')
			return 'A';

		return (char)(currentChar + 1);
	}
	private static void RegisterWithLogin(string login , string password,string passwordTwo)
	{

		bool loginExists = CheckLoginExists(login);

		if (loginExists)
		{
			Console.WriteLine("False");
			Log.Information("Логин уже существует: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		bool isValidLength = ValidateLength(login);
		if (!isValidLength)
		{
			Console.WriteLine("False");
			Log.Information("Логин должен содержать не менее 5 символов: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		bool isValidCyrillic = ValidateCyrillic(login);
		if (isValidCyrillic)
		{
			Console.WriteLine("False");
			Log.Information("Логин должен содержать только символы латиницы: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		bool isValidCharacters = ValidateCharacters(login);
		if (!isValidCharacters)
		{
			Console.WriteLine("False");
			Log.Information("Логин содержит недопустимые символы: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		PasswordTest(login,password, passwordTwo);

		
	}

	

	private static bool ValidateLength(string login)
	{
		return login.Length >= 5;
	}

	private static bool ValidateCyrillic(string login)
	{
		string cyrillicPattern = @"[\p{IsCyrillic}]";
		return Regex.IsMatch(login, cyrillicPattern);
	}

	private static bool ValidateCharacters(string login)
	{
		string validCharactersPattern = @"^[a-zA-Z0-9_]+$";
		return Regex.IsMatch(login, validCharactersPattern);
	}

	private static void RegisterWithPhoneNumber(string login , string password, string passwordTwo)
	{
		

		bool loginExists = CheckLoginExists(login);

		if (loginExists)
		{
			Console.WriteLine("False");
			Log.Information("Номер телефона уже существует: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		bool isValidatePhone = ValidatePhoneNumber(login);
		if (!isValidatePhone)
		{
			Console.WriteLine("False");
			Log.Information("Не соответсвует шаблону номера телефона (+7-xxx-xxx-xxxx): Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}
		else
			PasswordTest(login,password, passwordTwo);

	}

	private static bool ValidatePhoneNumber(string phoneNumber)
	{
		string phonePattern = @"^\+7-\d{3}-\d{3}-\d{4}$";

		if (Regex.IsMatch(phoneNumber, phonePattern))
		{
			return true; // Логин соответствует формату телефона
		}

		return false; // Логин не соответствует формату телефона
	}

	private static void RegisterWithEmail(string login, string password, string passwordTwo)
	{
		

		bool loginExists = CheckLoginExists(login);

		if (loginExists)
		{
			Console.WriteLine("False");
			Log.Information("Электронная почта уже существует: Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			return;
		}

		bool isValidateEmail = ValidateEmail(login);
		if (!isValidateEmail)
		{

			Console.WriteLine("False");
			Log.Information("Не соответсвует шаблону электронной почты (xxx@xxx.xxx): Дата-время запроса: {DateTime},\n" +
			" Логин: {Login},\n" +
			" Пароль: {MaskedPassword},\n" +
			" Подтверждение пароля: {MaskedConfPassword}", DateTime.Now, login, MaskPassword(password), MaskPassword(passwordTwo));
			RegistrationAcc();
			return;
		}
		else
			PasswordTest(login,password, passwordTwo);
	}
	private static bool ValidateEmail(string email)
	{
		string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

		if (Regex.IsMatch(email, emailPattern))
		{
			return true; // Логин соответствует формату электронной почты
		}

		return false; // Логин не соответствует формату электронной почты
	}
}
