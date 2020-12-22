using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.Project
{
    public class ProjectService : IProjectService
    {

        private IRepository<Entities.Project> _sysProjectRepository;


        public ProjectService(IRepository<Entities.Project> sysProjectRepository)
        {
            this._sysProjectRepository = sysProjectRepository;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.Project> searchProject(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysProjectRepository.Table;
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.ProjectCode.Contains(arg.pono));
            }
            query = query.OrderBy(o => o.ProjectCode);
            return new PagedList<Entities.Project>(query, page, size);
        }
        public Entities.Project getByAccount(string account)
        {
            return _sysProjectRepository.Table.FirstOrDefault(o => o.ProjectCode == account);
        }

        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysProjectRepository.Table.Any(o => o.ProjectCode == account );
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Project getById(int id)
        {
            return _sysProjectRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertProject(Entities.Project model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysProjectRepository.insert(model);
        }

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateProject(Entities.Project model)
        {
            _sysProjectRepository.DbContext.Entry(model).State = EntityState.Unchanged;
            _sysProjectRepository.DbContext.Entry(model).Property("ProjectCode").IsModified = true;
            _sysProjectRepository.DbContext.Entry(model).Property("Describe").IsModified = true;
            _sysProjectRepository.DbContext.SaveChanges();
        }
    }
}
