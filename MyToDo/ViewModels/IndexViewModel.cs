using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyToDo.ViewModels
{
    /// <summary>
    /// 首页ViewModel
    /// </summary>
    public class IndexViewModel : NavigationViewModel
    {
        private readonly IDialogHostService dialog;
        private readonly IToDoService toDoService;
        private readonly IMemoService memoService;
        private readonly IRegionManager regionManager;
        public IndexViewModel(IDialogHostService dialog,IContainerProvider provider) : base(provider)
        {
            Title = $"你好，{AppSession.UserName} {DateTime.Now.GetDateTimeFormats('D')[1].ToString()}";
            TaskBars = new ObservableCollection<TaskBar>();
            CreateTaskBars();
            ExecuteCommand = new DelegateCommand<string>(Execute); 
            this.toDoService = provider.Resolve<IToDoService>();
            this.memoService = provider.Resolve<IMemoService>();
            this.regionManager = provider.Resolve<IRegionManager>();
            this.dialog = dialog;
            EditToDoCommand = new DelegateCommand<ToDoDto>(AddToDo);
            EditMemoCommand = new DelegateCommand<MemoDto>(AddMemo);
            ToDoCompltedCommand = new DelegateCommand<ToDoDto>(Complted);
            NavigateCommand = new DelegateCommand<TaskBar>(Navigate);
        }

        private void Navigate(TaskBar obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Target)) return;
            NavigationParameters param = new NavigationParameters();
            if(obj.Title == "已完成")
            {
                param.Add("Value", 2);
            }
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.Target, param);
        }

        /// <summary>
        /// 待办事项状态修改
        /// </summary>
        /// <param name="obj"></param>
        private async void Complted(ToDoDto obj)
        {
            try
            {
                UpdateLoading(true);
                var updateResult = await toDoService.UpdateAsync(obj);
                if (updateResult.Status)
                {
                    var todo = SummaryDto.ToDoList.FirstOrDefault(t => t.Id.Equals(obj.Id));
                    if (todo != null)
                    {
                        SummaryDto.ToDoList.Remove(todo);
                        summaryDto.CompletedCount += 1;
                        SummaryDto.CompletedRatio = (SummaryDto.CompletedCount / (double)SummaryDto.Sum).ToString("0%");
                        this.Refresh();
                    }
                    aggregator.SendMessage("已完成!");
                }
            }
            finally
            {
                UpdateLoading(false);
            }
           
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增待办":AddToDo(null);break;
                case "新增备忘录":AddMemo(null);break;
            }
        }

        #region 属性

        //标题
        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }


        //任务栏
        private ObservableCollection<TaskBar> taskBars;
        public ObservableCollection<TaskBar> TaskBars
        {
            get { return taskBars; }
            set { taskBars = value; RaisePropertyChanged(); }
        }

        

        //首页统计
        private SummaryDto summaryDto;
        public SummaryDto SummaryDto
        {
            get { return summaryDto; }
            set { summaryDto = value; RaisePropertyChanged(); }
        }
        #endregion

        public DelegateCommand<string> ExecuteCommand { get; set; }

        /// <summary>
        /// 双击编辑待办事项
        /// </summary>
        public DelegateCommand<ToDoDto> EditToDoCommand { get; set; }

        /// <summary>
        /// 双击编辑备忘录
        /// </summary>
        public DelegateCommand<MemoDto> EditMemoCommand { get; set; }

        /// <summary>
        /// 待办事项状态编辑
        /// </summary>
        public DelegateCommand<ToDoDto> ToDoCompltedCommand { get; set; }

        /// <summary>
        /// 点击任务栏跳转页面
        /// </summary>
        public DelegateCommand<TaskBar> NavigateCommand { get; set; }



        void CreateTaskBars()
        {
            TaskBars.Add(new TaskBar() { Icon = "Podium", Color = "#FF0CA0FF", Title = "汇总", Target = "ToDoView" });
            TaskBars.Add(new TaskBar() { Icon = "ArchiveCheck", Color = "#FF1ECA3A", Title = "已完成", Target = "ToDoView" });
            TaskBars.Add(new TaskBar() { Icon = "Umbraco", Color = "#FF02C6DC", Title = "完成率", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "BookEdit", Color = "#FFFFA000", Title = "备忘录", Target = "MemoView" });
        }

        /// <summary>
        /// 添加待办事项
        /// </summary>
        async void AddToDo(ToDoDto model)
        {
            DialogParameters param = new DialogParameters();
            if (model != null)
                param.Add("Value", model);

            var dialogResult = await dialog.ShowDialog("AddToDoView",param);
            if(dialogResult.Result == ButtonResult.OK)
            {
                try
                {
                    UpdateLoading(true);
                    var todo = dialogResult.Parameters.GetValue<ToDoDto>("Value");
                    if (todo.Id > 0)
                    {
                        var updateResult = await toDoService.UpdateAsync(todo);
                        if (updateResult.Status)
                        {
                            var todoModel = SummaryDto.ToDoList.FirstOrDefault(t => t.Id.Equals(todo.Id));
                            if (todoModel != null)
                            {
                                todoModel.Title = todo.Title;
                                todoModel.Content = todo.Content;
                            }
                        }
                    }
                    else
                    {
                        var addResult = await toDoService.AddAsync(todo);
                        if (addResult.Status)
                        {
                            SummaryDto.Sum += 1;

                            SummaryDto.ToDoList.Add(addResult.Result);
                            SummaryDto.CompletedRatio = (SummaryDto.CompletedCount / (double)SummaryDto.Sum).ToString("0%");
                            this.Refresh();
                        }
                    }
                }
                finally
                {
                    UpdateLoading(false);
                }
                
            }
        }

        /// <summary>
        /// 添加备忘录
        /// </summary>
        async void AddMemo(MemoDto model)
        {
            DialogParameters param = new DialogParameters();
            if (model != null)
                param.Add("Value", model);

            var dialogResult = await dialog.ShowDialog("AddMemoView", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                try
                {
                    UpdateLoading(true);
                    var memo = dialogResult.Parameters.GetValue<MemoDto>("Value");
                    if (memo.Id > 0)
                    {
                        var updateResult = await memoService.UpdateAsync(memo);
                        if (updateResult.Status)
                        {
                            var memoModel = SummaryDto.MemoList.FirstOrDefault(t => t.Id.Equals(memo.Id));
                            if (memoModel != null)
                            {
                                memoModel.Title = memo.Title;
                                memoModel.Content = memo.Content;
                            }
                        }
                    }
                    else
                    {
                        var addResult = await memoService.AddAsync(memo);
                        if (addResult.Status)
                        {
                            SummaryDto.MemoList.Add(addResult.Result);
                            summaryDto.MemoCount += 1;
                            this.Refresh();
                        }
                    }
                }
                finally
                {
                    UpdateLoading(false);
                }
               
            }
        }

        /// <summary>
        /// 导航开始
        /// </summary>
        /// <param name="navigationContext"></param>
        public async override void OnNavigatedTo(NavigationContext navigationContext)
        {
            UpdateLoading(true);
            var summaryResult = await toDoService.SummaryAsync();
            if (summaryResult.Status)
            {
                SummaryDto = summaryResult.Result;
                Refresh();
            }
            base.OnNavigatedTo(navigationContext);
            UpdateLoading(false);
        }

        void Refresh()
        {
            TaskBars[0].Content = SummaryDto.Sum.ToString();
            TaskBars[1].Content = SummaryDto.CompletedCount.ToString();
            TaskBars[2].Content = SummaryDto.CompletedRatio;
            TaskBars[3].Content = SummaryDto.MemoCount.ToString();
        }

    }
}
