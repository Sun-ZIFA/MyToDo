using MyToDo.Common;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace MyToDo.ViewModels.Dialogs
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        private readonly ILoginService service;
        private readonly IEventAggregator aggregator;
        public LoginViewModel(ILoginService service, IEventAggregator aggregator)
        {
            UserDto = new RegisterUserDto();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.service = service;
            this.aggregator = aggregator;
        }

        private void Execute(string arg)
        {
            switch (arg)
            {
                //登录系统
                case "Login": Login(); break;
                //退出登录
                case "LoginOut": LoginOut(); break;
                //跳转注册页面
                case "Go": SelectedIndex = 1; break;
                //注册账号
                case "Register": Register(); break;
                //返回登录页面
                case "Return": SelectedIndex = 0; break;
            }
        }

        private async void Register()
        {
            if (string.IsNullOrWhiteSpace(UserDto.UserName) || string.IsNullOrWhiteSpace(UserDto.Account) || string.IsNullOrWhiteSpace(UserDto.Password) || string.IsNullOrWhiteSpace(UserDto.NewPassword))
                return;
            if (UserDto.Password != UserDto.NewPassword)
            {
                //验证失败提示
                aggregator.SendMessage("输入密码不一致", "Login");
                return;
            }
                
            var registerResult = await service.RegisterAsync(new Shared.Dtos.UserDto()
            {
                Account = UserDto.Account,
                UserName = UserDto.UserName,
                Password = UserDto.Password
            });
            if (registerResult != null && registerResult.Status)
            {
                //注册成功
                aggregator.SendMessage("注册成功","Login");
                SelectedIndex = 0;
                return;
            }
            //注册失败提示...
            aggregator.SendMessage(registerResult.Message, "Login");
        }

        private void LoginOut()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private async void Login()
        {
            if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password))
                return;
            var loginResult = await service.LoginAsync(new Shared.Dtos.UserDto()
            {
                Account = Account,
                Password = Password,
            });
            if (loginResult.Status)
            {
                AppSession.UserName = loginResult.Result.UserName;
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                return;
            }

            //登录失败提示
            aggregator.SendMessage(loginResult.Message, "Login");
        }

        public string Title { get; set; } = "ToDo";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            LoginOut();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        public DelegateCommand<string> ExecuteCommand { get; set; }

        private string account;

        public string Account
        {
            get { return account; }
            set { account = value; RaisePropertyChanged(); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }

        private RegisterUserDto userDto;

        public RegisterUserDto UserDto
        {
            get { return userDto; }
            set { userDto = value; RaisePropertyChanged(); }
        }


    }
}
