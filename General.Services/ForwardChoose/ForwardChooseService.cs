using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.ForwardChoose
{
    public class ForwardChooseService : IForwardChooseService
    {

        private IRepository<Entities.ForwardChoose> _sysForwardChooseRepository;


        public ForwardChooseService(IRepository<Entities.ForwardChoose> sysForwardChooseRepository)
        {
            this._sysForwardChooseRepository = sysForwardChooseRepository;
        }
        public Entities.ForwardChoose getByAccount(string account)
        {
            return _sysForwardChooseRepository.Table.FirstOrDefault(o => o.Transport == account);
        }
        public Entities.ForwardChoose getForwarda(string a)
        {
            return _sysForwardChooseRepository.Table.FirstOrDefault(o => o.Dest == a );
        }
        public Entities.ForwardChoose getForwardb(string a, string b)
        {
            return _sysForwardChooseRepository.Table.FirstOrDefault(o => o.Transport == a  && o.Dest == b);
        }
        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.ForwardChoose> searchForwardChoose(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysForwardChooseRepository.Table;
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.keyword))
                    query = query.Where(o => o.Forward==arg.keyword);
            }
            query = query.OrderBy(o => o.Dest);
            return new PagedList<Entities.ForwardChoose>(query, page, size);
        }
        public List<Entities.ForwardChoose> getCount(string a, string b)
        {
            List<Entities.ForwardChoose> list = null;

            if (list != null)
                return list;

            list = _sysForwardChooseRepository.Table.Where(o => o.Forward == a && o.Dest != b).ToList();


            return list;
        }
        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string a,string b)
        {
            return _sysForwardChooseRepository.Table.Any(o =>  o.Dest == a && o.Forward == b);
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.ForwardChoose getById(int id)
        {
            return _sysForwardChooseRepository.getById(id);
        }
        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertForwardChoose(Entities.ForwardChoose model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysForwardChooseRepository.insert(model);
        }
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateForwardChoose(Entities.ForwardChoose model)
        {
            var item = _sysForwardChooseRepository.getById(model.Id);
            if (item == null)
                return;
            _sysForwardChooseRepository.update(model);
        }
    }
}
