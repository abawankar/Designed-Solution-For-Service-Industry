
using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Task;
using Domain.Implementation.Task;
using Domain.Interface.Task;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.ATM.Models
{
    public class TaskManagerModel : Domain.Implementation.Task.TaskManager
    {
        public int AssignerId { get; set; }
        public int LocationId { get; set; }
        public int StartH { get; set; }
        public int StartM { get; set; }
        public int DueH { get; set; }
        public int DueM { get; set; }
        public int ComplH { get; set; }
        public int ComplM { get; set; }
        public int IsMail { get; set; }
        public int IsReport { get; set; }
        public string Assignerlist { get; set; }
        public string externalfile { get; set; }

        public int actualH { get; set; }
        public int actualM { get; set; }
    }

    public class TaskManagerRepository : Repository<TaskManagerModel>
    {

        public override TaskManagerModel GetById(int id)
        {
            TaskManagerDAL dal = new TaskManagerDAL();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>()
                .ForMember(dest => dest.AssignerId, opt => opt.MapFrom(scr => scr.Assigneer.Id))
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(scr => scr.Location.Id));
            TaskManagerModel model = AutoMapper.Mapper.Map<TaskManagerModel>(dal.GetById(id));

            return model;
        }

        public override IList<TaskManagerModel> GetAll()
        {
            TaskManagerDAL dal = new TaskManagerDAL();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>()
                .ForMember(dest => dest.AssignerId, opt => opt.MapFrom(scr => scr.Assigneer.Id))
            .ForMember(dest => dest.LocationId, opt => opt.MapFrom(scr => scr.Location.Id));
            List<TaskManagerModel> model = AutoMapper.Mapper.Map<List<TaskManagerModel>>(dal.GetAll());

            return model;
        }

        public IList<TaskManagerModel> GetTodayTask(string name,string date)
        {
            EmployeeRepository e = new EmployeeRepository();
            int empid = IdentityExtensions.AppsUser.Id; //e.GetByName(name);
            TaskManagerDAL dal = new TaskManagerDAL();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>()
                .ForMember(dest => dest.AssignerId, opt => opt.MapFrom(scr => scr.Assigneer.Id))
            .ForMember(dest => dest.LocationId, opt => opt.MapFrom(scr => scr.Location.Id));
            List<TaskManagerModel> model = AutoMapper.Mapper.Map<List<TaskManagerModel>>(dal.GetTodayTask(date).ToList());

            model = model.Where(x => x.Assigneer.Id == empid || x.Contacts.Any(y => y.Id == empid)).ToList();
            return model;
        }

        public IList<TaskManagerModel> GetByStatus(int statusid)
        {
            TaskManagerDAL dal = new TaskManagerDAL();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>()
                .ForMember(dest => dest.AssignerId, opt => opt.MapFrom(scr => scr.Assigneer.Id));
            List<TaskManagerModel> model = AutoMapper.Mapper.Map<List<TaskManagerModel>>(dal.GetByStatus(statusid));

            return model;
        }

        public IList<TaskManagerModel> GetByEmployee(string name)
        {
            EmployeeRepository e = new EmployeeRepository();
            TaskManagerDAL dal = new TaskManagerDAL();
            int empid = IdentityExtensions.AppsUser.Id;// e.GetByName(name);
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>()
                .ForMember(dest => dest.AssignerId, opt => opt.MapFrom(scr => scr.Assigneer.Id));
            List<TaskManagerModel> model = AutoMapper.Mapper.Map<List<TaskManagerModel>>(dal.GetAllByEmp(empid).ToList());

            return model;
        }

        public IList<TaskManagerModel> GetAllByEmp(string name)
        {
            EmployeeRepository e = new EmployeeRepository();
            TaskManagerDAL dal = new TaskManagerDAL();
            int empid = IdentityExtensions.AppsUser.Id;// e.GetByName(name);
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>();
            AutoMapper.Mapper.CreateMap<TaskManager, TaskManagerModel>()
                .ForMember(dest => dest.AssignerId, opt => opt.MapFrom(scr => scr.Assigneer.Id));
            List<TaskManagerModel> model = AutoMapper.Mapper.Map<List<TaskManagerModel>>(dal.GetAllByEmp(empid).ToList());

            return model;
        }

        public IList<TaskManagerModel> GetByBranch(int branch)
        {
            List<TaskManagerModel> model = new List<TaskManagerModel>();
            model = GetAll().Where(x => x.Assigneer.Branch.Id == branch).ToList();
            return model;
        }

        public override void Edit(TaskManagerModel obj)
        {
            TaskManagerDAL dal = new TaskManagerDAL();
            ITaskManager bl = dal.GetById(obj.Id);
            bl.Type = obj.Type;
            bl.Task = obj.Task;
            bl.Notes = obj.Notes;
            bl.Start = Convert.ToDateTime(obj.Start);// + DateTime.Now.TimeOfDay;
            bl.StartTime = obj.StartTime;
            bl.Due = Convert.ToDateTime(obj.Due);// + DateTime.Now.TimeOfDay;
            bl.DueTime = obj.DueTime;
            bl.Compl = Convert.ToDateTime(obj.Compl);// + DateTime.Now.TimeOfDay;
            bl.ComplTime = obj.ComplTime;
            bl.Status = obj.Status;
            if (obj.Status == 3)
                bl.CompletePercentage = "100%";
            else
                bl.CompletePercentage = obj.CompletePercentage;
            bl.ActualHours = obj.ActualHours;

            TimeSpan duration = DateTime.Parse(obj.ComplTime).Subtract(DateTime.Parse(obj.StartTime));
            bl.TotalHours = duration.ToString();
            bl.EmpCost = bl.EmpCost == null ? 0 : bl.EmpCost;
            double total = 0;
            if (!string.IsNullOrEmpty(obj.ActualHours))
            {
                string[] actual = obj.ActualHours.Split(':');
                total = Convert.ToDouble((bl.EmpCost * Convert.ToDouble(actual[0])) + (bl.EmpCost * (Convert.ToDouble(actual[1]) == 0 ? 0 : Convert.ToDouble(actual[1]) / 60)));
            }


            bl.TotalCost = total;
            bl.OtherCost = obj.OtherCost == null ? 0 : obj.OtherCost;
            bl.GrandTotal = total + bl.OtherCost;
            List<IRepeatTask> repeat = new List<IRepeatTask>();
            if (obj.TaskRepeat != 0 || obj.TaskRepeat != 5)
            {
                if (bl.TaskRepeat != obj.TaskRepeat)
                {
                    switch (obj.TaskRepeat)
                    {
                        case 1:
                            repeat = RepeatTaskDaily(bl.Start, Convert.ToDateTime(bl.Due));
                            break;
                        case 2:
                            repeat = RepeatTaskWeekly(bl.Start, Convert.ToDateTime(bl.Due));
                            break;
                        case 3:
                            repeat = RepeatTaskMonthly(bl.Start, Convert.ToDateTime(bl.Due));
                            break;
                        case 4:
                            repeat = RepeatTaskYearly(bl.Start, Convert.ToDateTime(bl.Due));
                            break;
                        default:
                            break;
                    }
                    bl.RepeatTask = repeat;
                }
            }
            bl.TaskRepeat = obj.TaskRepeat;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(TaskManagerModel obj)
        {
            TaskManagerDAL dal = new TaskManagerDAL();
            ITaskManager bl = new TaskManager();
            bl.Date = Convert.ToDateTime(obj.Date) + DateTime.Now.TimeOfDay;
            bl.Type = obj.Type;
            bl.Task = obj.Task;
            bl.Notes = obj.Notes;
            bl.Start = Convert.ToDateTime(obj.Start) + DateTime.Now.TimeOfDay;
            bl.StartTime = obj.StartTime;
            bl.Due = Convert.ToDateTime(obj.Due) + DateTime.Now.TimeOfDay;
            bl.DueTime = obj.DueTime;
            bl.Compl = Convert.ToDateTime(obj.Compl) + DateTime.Now.TimeOfDay;
            bl.ComplTime = obj.ComplTime;
            bl.JobNumber = obj.JobNumber;
            bl.CompletePercentage = obj.CompletePercentage;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            TaskManagerDAL dal = new TaskManagerDAL();
            ITaskManager bl = dal.GetById(id);
            return dal.Delete(bl);
        }

        public IList<RepeatTaskModel> RepeatTaskList(int taskid)
        {
            TaskManagerDAL dal = new TaskManagerDAL();
            AutoMapper.Mapper.CreateMap<RepeatTask, RepeatTaskModel>();
            List<RepeatTaskModel> model = AutoMapper.Mapper.Map<List<RepeatTaskModel>>(dal.GetById(taskid).RepeatTask.ToList());

            return model;
        }

        private List<IRepeatTask> RepeatTaskDaily(DateTime startDate, DateTime dueDate)
        {
            int days = (dueDate - startDate).Days;
            List<IRepeatTask> task = new List<IRepeatTask>();
            for (int i = 1; i < days + 1; i++)
            {
                IRepeatTask bl = new RepeatTask();
                bl.TaskDate = startDate.AddDays(i);
                bl.Status = 1;
                task.Add(bl);
            }
            return task;
        }

        private List<IRepeatTask> RepeatTaskWeekly(DateTime startDate, DateTime dueDate)
        {
            int days = (dueDate - startDate).Days;
            List<IRepeatTask> task = new List<IRepeatTask>();
            for (int i = 7; i < days + 1; i += 7)
            {
                IRepeatTask bl = new RepeatTask();
                bl.TaskDate = startDate.AddDays(i);
                bl.Status = 1;
                task.Add(bl);
            }
            return task;
        }

        private List<IRepeatTask> RepeatTaskMonthly(DateTime startDate, DateTime dueDate)
        {
            int days = (dueDate - startDate).Days;
            int month = days / 30;
            List<IRepeatTask> task = new List<IRepeatTask>();
            for (int i = 1; i < month + 1; i += 1)
            {
                IRepeatTask bl = new RepeatTask();
                bl.TaskDate = startDate.AddMonths(i);
                bl.Status = 1;
                task.Add(bl);
            }
            return task;
        }

        private List<IRepeatTask> RepeatTaskYearly(DateTime startDate, DateTime dueDate)
        {
            int days = (dueDate - startDate).Days;
            int year = days / 360;
            List<IRepeatTask> task = new List<IRepeatTask>();
            for (int i = 1; i < year + 1; i += 1)
            {
                IRepeatTask bl = new RepeatTask();
                bl.TaskDate = startDate.AddYears(i);
                bl.Status = 1;
                task.Add(bl);
            }
            return task;
        }

    }
}