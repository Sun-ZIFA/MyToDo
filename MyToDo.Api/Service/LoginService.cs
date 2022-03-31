using AutoMapper;
using MyToDo.Api.Context;
using MyToDo.Api.Context.UnitOfWork;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace MyToDo.Api.Service
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork work;
        private readonly IMapper mapper;

        public LoginService(IUnitOfWork work,IMapper mapper)
        {
            this.work = work;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> LoginAsync(string account, string password)
        {
            try
            {
                password = password.GetMD5();
                var Repository = await work.GetRepository<User>().GetFirstOrDefaultAsync(predicate: u => u.Account.Equals(account) && u.PassWord.Equals(password));
                if (Repository == null)
                {
                    return new ApiResponse("账号密码错误，请重新输入");
                }
                return new ApiResponse(true, Repository);
            }
            catch (Exception ex)
            {

                return new ApiResponse(false, "登录失败");
            }
        }

        public async Task<ApiResponse> RegisterAsync(UserDto user)
        {
            try
            {
                var model =  mapper.Map<User>(user);
                var repository = work.GetRepository<User>();
                var isUser = await repository.GetFirstOrDefaultAsync(predicate: u => u.Account.Equals(model.Account));
                if(isUser != null)
                {
                    return new ApiResponse($"当前账号：{model.Account}已存在，请重新输入！");
                }

                model.CreateDate = DateTime.Now;
                model.PassWord = model.PassWord.GetMD5();
                await repository.InsertAsync(model);

                if (await work.SaveChangesAsync() > 0)
                {
                    return new ApiResponse(true, model);
                }
                return new ApiResponse("注册账号失败，请稍后重试！");
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, "注册账号失败");
            }
        }
    }
}
