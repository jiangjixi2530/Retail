using SqlSugar;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Retail.DAL.Repository
{
    #region SqlSugar仓储层基类

    public class SugarHandler : ISugarHandler
    {
        #region 属性 
        /// <summary>
        /// 当前数据库连接
        /// </summary>
        public string CurrentConnect { get; set; }
        /// <summary>
        /// 数据库上下文
        /// </summary>
        protected SqlSugarClient DbContext { get; set; }

        /// <summary>
        /// 当前操作执行的SQL语句
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SugarHandler()
        {
            //Sql = string.Empty;
            Init();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化DB
        /// </summary>
        private void Init()
        {
            //var dbContext = CallContext.GetData("DBContext") as BaseDbContext;
            //if (dbContext == null)
            //{
            //    dbContext = new BaseDbContext();
            //    CallContext.SetData("DBContext", dbContext);
            //}
            //DbContext = dbContext.Db;
            DbContext = new BaseDbContext(Util.AppSetting.DbConnectionString).Db;
        }
        #endregion

        #region 事务

        /// <summary>
        /// 初始化事务
        /// </summary>
        /// <param name="level"></param>
        public void BeginTran(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            DbContext.Ado.BeginTran(IsolationLevel.Unspecified);
        }

        /// <summary>
        /// 完成事务
        /// </summary>
        public void CommitTran()
        {
            DbContext.Ado.CommitTran();
        }

        /// <summary>
        /// 完成事务
        /// </summary>
        public void RollbackTran()
        {
            DbContext.Ado.RollbackTran();
        }
        #endregion

        #region 新增 

        /// <summary>
        /// 新增数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public IInsertable<T> Insertable<T>(T entity) where T : class, new()
        {
            return DbContext.Insertable<T>(entity);
        }
        public IInsertable<T> Insertable<T>(List<T> list) where T : class, new()
        {
            return DbContext.Insertable<T>(list);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>新增是否成功</returns>
        public bool Add<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entitys">泛型集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>新增是否成功</returns>
        public bool Add<T>(List<T> entitys, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                 DbContext.Insertable(entitys).With(SqlWith.UpdLock)
                 : DbContext.Insertable(entitys);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回实体</returns>
        public T AddReturnEntity<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteReturnEntity();
            return result;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回Id</returns>
        public int AddReturnId<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteReturnIdentity();
            return result;

        }

        /// <summary>
        /// 新增
        /// </summary> 
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        public bool AddReturnBool<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteCommandIdentityIntoEntity();
            return result;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entitys">泛型集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        public bool AddReturnBool<T>(List<T> entitys, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Insertable(entitys).With(SqlWith.UpdLock)
                : DbContext.Insertable(entitys);
            var result = operate.ExecuteCommandIdentityIntoEntity();
            return result;
        }

        #endregion

        #region 修改 

        /// <summary>
        /// 修改数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public IUpdateable<T> Updateable<T>() where T : class, new()
        {
            return DbContext.Updateable<T>();
        }

        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Updateable(entity).With(SqlWith.UpdLock)
                : DbContext.Updateable(entity);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="update"> 实体对象 </param> 
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update<T>(Expression<Func<T, T>> update, Expression<Func<T, bool>> where, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Updateable<T>().UpdateColumns(update).Where(where).With(SqlWith.UpdLock)
                : DbContext.Updateable<T>().UpdateColumns(update).Where(where);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="update"> 匿名对象 </param> 
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update<T>(Expression<Func<T, object>> update, Expression<Func<T, bool>> where, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Updateable<T>().UpdateColumns(update).Where(where).With(SqlWith.UpdLock)
               : DbContext.Updateable<T>().UpdateColumns(update).Where(where);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entitys"> 实体对象集合 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update<T>(List<T> entitys, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Updateable(entitys).With(SqlWith.UpdLock)
                : DbContext.Updateable(entitys);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        #endregion

        #region 删除

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="list"> 实体对象列表 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool Delete<T>(List<T> list, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Deleteable(list).With(SqlWith.UpdLock)
                : DbContext.Deleteable(list);
            var result = operate.ExecuteCommand();
            return result > 0;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool Delete<T>(T entity, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
               DbContext.Deleteable(entity).With(SqlWith.UpdLock)
                : DbContext.Deleteable(entity);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool Delete<T>(Expression<Func<T, bool>> where, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                    DbContext.Deleteable<T>().Where(where).With(SqlWith.UpdLock)
                   : DbContext.Deleteable<T>().Where(where);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 删除所有
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool DeleteAll<T>(bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                    DbContext.Deleteable<T>().With(SqlWith.UpdLock)
                   : DbContext.Deleteable<T>();
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 根据主键物理删除实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool DeleteById<T>(dynamic id, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                  DbContext.Deleteable<T>().In(id).With(SqlWith.UpdLock)
                 : DbContext.Deleteable<T>().In(id);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 根据主键批量物理删除实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids">主键集合</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool DeleteByIds<T>(dynamic[] ids, bool isLock = false) where T : class, new()
        {
            var operate = isLock ?
                DbContext.Deleteable<T>().In(ids).With(SqlWith.UpdLock)
                : DbContext.Deleteable<T>().In(ids);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        #endregion

        #region 查询

        /// <summary>
        /// 查询数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public ISugarQueryable<T> Queryable<T>() where T : class, new()
        {
            return DbContext.Queryable<T>();
        }

        /// <summary>
        /// 多表连接查询数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public ISugarQueryable<T, T2> Queryable<T, T2>(Expression<Func<T, T2, object[]>> joinExpression)
        {
            return DbContext.Queryable<T, T2>(joinExpression);
        }

        /// <summary>
        /// 多表连接查询数据源
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <returns>数据源</returns>
        public ISugarQueryable<T, T2, T3> Queryable<T, T2, T3>(Expression<Func<T, T2, T3, object[]>> joinExpression)
        {
            return DbContext.Queryable<T, T2, T3>(joinExpression);
        }
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="whereLambda">查询表达式</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>实体</returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            if (orderbyLambda != null)
            {
                query = query.OrderBy(orderbyLambda, isAsc ? OrderByType.Asc : OrderByType.Desc);
            }
            return query.ToList();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <returns>实体</returns>
        public List<T> GetList<T>() where T : class, new()
        {
            var query = Queryable<T>();
            return query.ToList();
        }


        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体</returns> 
        public List<T> GetList<T>(string sql, object parameters) where T : class, new()
        {
            return DbContext.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体</returns>
        public List<T> GetList<T>(string sql, params SugarParameter[] parameters) where T : class, new()
        {
            return DbContext.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 根据条件获取实体列表
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="conditionals">Sugar调价表达式集合</param>
        /// <returns>实体</returns>
        public List<T> GetList<T>(List<IConditionalModel> conditionals) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(conditionals);
            return query.ToList();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="whereLambda">查询表达式</param>
        /// <returns>实体</returns>
        public DataTable GetDataTable<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            return query.ToDataTable();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="sql">sql</param>
        /// <returns>实体</returns>
        public DataTable GetDataTable<T>(string sql) where T : class, new()
        {
            var query = DbContext.SqlQueryable<T>(sql).With(SqlWith.NoLock);
            return query.ToDataTable();
        }
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            return DbContext.Ado.GetDataTable(sql);
        }
        /// <summary>
        /// 查询存储过程
        /// </summary> 
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">参数</param>
        public DataTable GetProcedure(string procedureName, List<SugarParameter> parameters)
        {
            var datas = DbContext.Ado.UseStoredProcedure().GetDataTable(procedureName, parameters);
            return datas;
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <param name="whereLambda">查询表达式</param> 
        /// <param name="orderbyLambda">排序表达式</param> 
        /// <param name="isAsc">是否升序</param> 
        /// <returns></returns>
        public T Single<T>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);

            if (orderbyLambda != null)
            {
                query = query.OrderBy(orderbyLambda, isAsc ? OrderByType.Asc : OrderByType.Desc);
            }

            var rs = query.First();

            return rs;
        }

        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById<T>(dynamic id) where T : class, new()
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock);

            return query.InSingle(id);
        }

        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public object GetScalar(string sql, object parameters)
        {
            return DbContext.Ado.GetScalar(sql, parameters);
        }

        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public object GetScalar(string sql, params SugarParameter[] parameters)
        {
            return DbContext.Ado.GetScalar(sql, parameters);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型</typeparam>
        /// <param name="whereLambda">查询表达式</param> 
        /// <returns></returns>
        public bool IsExist<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            var datas = DbContext.Queryable<T>().Any(whereLambda);
            return datas;
        }
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        public int ExecuteSql(string sql)
        {
            return DbContext.Ado.ExecuteCommand(sql);
        }
        #endregion

        #region 分页查询

        /// <summary>
        /// 获取分页列表【页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList<T>(int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>();
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【排序，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList<T>(Expression<Func<T, object>> orderExp, OrderByType orderType, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().OrderBy(orderExp, orderType);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【Linq表达式条件，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList<T>(Expression<Func<T, bool>> whereExp, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(whereExp);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【Linq表达式条件，排序，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList<T>(Expression<Func<T, bool>> whereExp, Expression<Func<T, object>> orderExp, OrderByType orderType, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(whereExp).OrderBy(orderExp, orderType);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【Sugar表达式条件，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="conditionals">Sugar条件表达式集合</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList<T>(List<IConditionalModel> conditionals, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(conditionals);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        ///  获取分页列表【Sugar表达式条件，排序，页码，每页条数】
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="conditionals">Sugar条件表达式集合</param>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList<T>(List<IConditionalModel> conditionals, Expression<Func<T, object>> orderExp, OrderByType orderType, int pageIndex, int pageSize) where T : class, new()
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(conditionals).OrderBy(orderExp, orderType);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }


        #endregion

        #region 执行sql

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        //public object ExecuteSql(string sql)
        //{
        //    var obj = DbContext.Ado.ExecuteCommand(sql);
        //    return obj;
        //}

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteSql(string sql, object parameters)
        {
            var obj = DbContext.Ado.ExecuteCommand(sql, parameters);
            return obj;
        }

        #endregion
        #region 私有方法

        /// <summary>
        /// 查询条件转换
        /// </summary>
        /// <param name="contitons">查询条件</param>
        /// <returns></returns>
        protected List<IConditionalModel> ParseCondition(List<ConditionalModel> contitons)
        {
            var conds = new List<IConditionalModel>();
            foreach (var con in contitons)
            {
                if (con.FieldName.Contains(","))
                {
                    conds.Add(ParseKeyOr(con));
                }
                else
                {
                    conds.Add(new ConditionalModel()
                    {
                        FieldName = con.FieldName,
                        ConditionalType = con.ConditionalType,
                        FieldValue = con.FieldValue
                    });
                }
            }

            return conds;
        }

        /// <summary>
        /// 转换Or条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected ConditionalCollections ParseKeyOr(ConditionalModel condition)
        {
            var objectKeys = condition.FieldName.Split(',');
            var conditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>();
            foreach (var objKey in objectKeys)
            {
                var cond = new KeyValuePair<WhereType, ConditionalModel>
                (WhereType.Or, new ConditionalModel()
                {
                    FieldName = objKey,
                    ConditionalType = condition.ConditionalType,
                    FieldValue = condition.FieldValue
                });
                conditionalList.Add(cond);
            }
            return new ConditionalCollections { ConditionalList = conditionalList };
        }


        /// <summary>
        /// 根据SQLSugar表达式获取Sql语句
        /// </summary>
        /// <param name="keyValuePair"></param>
        /// <returns></returns>
        protected string GetSql(KeyValuePair<string, List<SugarParameter>> keyValuePair)
        {
            var sql = keyValuePair.Key;
            foreach (var para in keyValuePair.Value)
            {
                if (sql.IndexOf(para.ParameterName + ",") >= 0)
                    sql = sql.Replace(para.ParameterName + ",", GetSqlValue(para.Value) + ",");
                else
                    sql = sql.Replace(para.ParameterName, GetSqlValue(para.Value));
            }
            return sql;
        }

        /// <summary>
        /// 值转成sql值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetSqlValue(object value)
        {

            if (value == null || Convert.IsDBNull(value))
                return string.Empty;
            if (value is string)
            {
                return string.Format("'{0}'", value.ToString());
            }
            else if (value is DateTime)
            {
                DateTime time = (DateTime)value;
                return string.Format("'{0}'", time.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else if (value is bool)
            {
                return Convert.ToBoolean(value) ? "1" : "0";
            }
            else
            {
                return value.ToString();
            }
        }

        #endregion

        /// <summary>
        /// 处理
        /// </summary>
        public void Dispose()
        {

            DbContext.Close();
        }


    }


    #endregion

    #region SqlSugar仓储层泛型基类

    public class SugarHandler<T> : SugarHandler, ISugarHandler<T> where T : class, new()
    {
        #region 新增 

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>新增是否成功</returns>
        public bool Add(T entity, bool isLock = false)
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entitys">泛型集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>新增是否成功</returns>
        public bool Add(List<T> entitys, bool isLock = false)
        {
            var operate = isLock ?
                 DbContext.Insertable(entitys).With(SqlWith.UpdLock)
                 : DbContext.Insertable(entitys);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回实体</returns>
        public T AddReturnEntity(T entity, bool isLock = false)
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteReturnEntity();
            return result;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回Id</returns>
        public int AddReturnId(T entity, bool isLock = false)
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteReturnIdentity();
            return result;
        }

        /// <summary>
        /// 新增
        /// </summary> 
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        public bool AddReturnBool(T entity, bool isLock = false)
        {
            var operate = isLock ?
                DbContext.Insertable(entity).With(SqlWith.UpdLock)
                : DbContext.Insertable(entity);
            var result = operate.ExecuteCommandIdentityIntoEntity();
            return result;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entitys">泛型集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>返回bool, 并将identity赋值到实体</returns>
        public bool AddReturnBool(List<T> entitys, bool isLock = false)
        {
            var operate = isLock ?
                DbContext.Insertable(entitys).With(SqlWith.UpdLock)
                : DbContext.Insertable(entitys);
            var result = operate.ExecuteCommandIdentityIntoEntity();
            return result;
        }

        #endregion

        #region 修改 

        /// <summary>
        /// 修改数据源
        /// </summary>
        /// <returns>数据源</returns>
        public IUpdateable<T> Updateable()
        {
            return DbContext.Updateable<T>();
        }

        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update(T entity, bool isLock = false)
        {
            var operate = isLock ?
                DbContext.Updateable(entity).With(SqlWith.UpdLock)
                : DbContext.Updateable(entity);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="update"> 实体对象 </param> 
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update(Expression<Func<T, T>> update, Expression<Func<T, bool>> where, bool isLock = false)
        {
            var operate = isLock ?
                DbContext.Updateable<T>().UpdateColumns(update).Where(where).With(SqlWith.UpdLock)
                : DbContext.Updateable<T>().UpdateColumns(update).Where(where);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="update"> 匿名对象 </param> 
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update(Expression<Func<T, object>> update, Expression<Func<T, bool>> where, bool isLock = false)
        {
            var operate = isLock ?
                DbContext.Updateable<T>().UpdateColumns(update).Where(where).With(SqlWith.UpdLock)
                : DbContext.Updateable<T>().UpdateColumns(update).Where(where);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 修改（主键是更新条件）
        /// </summary>
        /// <param name="entitys"> 实体对象集合 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>修改是否成功</returns>
        public bool Update(List<T> entitys, bool isLock = false)
        {
            var operate = isLock ?
               DbContext.Updateable(entitys).With(SqlWith.UpdLock)
                : DbContext.Updateable(entitys);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        #endregion

        #region 删除


        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">泛型参数(集合成员的类型)</typeparam>
        /// <param name="list"> 实体对象列表 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool Delete(List<T> list, bool isLock = false)
        {
            var operate = isLock
                ? DbContext.Deleteable(list).With(SqlWith.UpdLock)
                : DbContext.Deleteable(list);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"> 实体对象 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool Delete(T entity, bool isLock = false)
        {
            var operate = isLock ?
               DbContext.Deleteable(entity).With(SqlWith.UpdLock)
                : DbContext.Deleteable(entity);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="where"> 条件 </param> 
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool Delete(Expression<Func<T, bool>> where, bool isLock = false)
        {
            var operate = isLock ?
                    DbContext.Deleteable<T>().Where(where).With(SqlWith.UpdLock)
                   : DbContext.Deleteable<T>().Where(where);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool DeleteAll(bool isLock = false)
        {
            var operate = isLock ?
                    DbContext.Deleteable<T>().With(SqlWith.UpdLock)
                   : DbContext.Deleteable<T>();
            var result = operate.ExecuteCommand();

            return result > 0;
        }
        /// <summary>
        /// 根据主键物理删除实体对象
        /// </summary>
        /// <param name="id">主键</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool DeleteById(dynamic id, bool isLock = false)
        {
            var operate = isLock ?
                  DbContext.Deleteable<T>().In(id).With(SqlWith.UpdLock)
                 : DbContext.Deleteable<T>().In(id);
            var result = operate.ExecuteCommand();
            return result > 0;
        }

        /// <summary>
        /// 根据主键批量物理删除实体集合
        /// </summary>
        /// <param name="ids">主键集合</param>
        ///<param name="isLock"> 是否加锁 </param> 
        /// <returns>删除是否成功</returns>
        public bool DeleteByIds(dynamic[] ids, bool isLock = false)
        {
            var operate = isLock ?
                DbContext.Deleteable<T>().In(ids).With(SqlWith.UpdLock)
                : DbContext.Deleteable<T>().In(ids);
            var result = operate.ExecuteCommand();

            return result > 0;
        }

        #endregion

        #region 查询

        /// <summary>
        /// 查询数据源
        /// </summary>
        /// <returns>数据源</returns>
        public ISugarQueryable<T> Queryable()
        {
            return DbContext.Queryable<T>();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <returns>实体</returns>
        public List<T> GetList()
        {
            var query = Queryable();
            return query.ToList();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <param name="orderbyLambda">排序表达式</param>
        /// <param name="isAsc">查询表达式</param>
        /// <returns>实体</returns>
        public List<T> GetList(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true)
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            if (orderbyLambda != null)
            {
                query = query.OrderBy(orderbyLambda, isAsc ? OrderByType.Asc : OrderByType.Desc);
            }
            return query.ToList();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体</returns> 
        public List<T> GetList(string sql, object parameters)
        {
            return DbContext.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <returns>实体</returns>
        public List<T> GetSugarList(string sql, SugarParameter[] parameters)
        {
            var para = new List<SugarParameter>(parameters);
            return DbContext.Ado.SqlQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 根据条件获取实体列表
        /// </summary>
        /// <param name="conditionals">Sugar调价表达式集合</param>
        /// <returns>实体</returns>
        public List<T> GetList(List<IConditionalModel> conditionals)
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(conditionals);
            return query.ToList();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <returns>实体</returns>
        public DataTable GetDataTable(Expression<Func<T, bool>> whereLambda)
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            return query.ToDataTable();
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>实体</returns>
        public DataTable GetDataTable(string sql)
        {
            var query = DbContext.SqlQueryable<T>(sql).With(SqlWith.NoLock);
            return query.ToDataTable();
        }


        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="whereLambda">查询表达式</param> 
        /// <param name="orderbyLambda">排序表达式</param> 
        /// <param name="isAsc">是否升序</param> 
        /// <returns></returns>
        public T Single(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderbyLambda = null, bool isAsc = true)
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock).Where(whereLambda);
            if (orderbyLambda != null)
            {
                query = query.OrderBy(orderbyLambda, isAsc ? OrderByType.Asc : OrderByType.Desc);
            }
            var rs = query.First();
            return rs;
        }

        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById(dynamic id)
        {
            var query = DbContext.Queryable<T>().With(SqlWith.NoLock);
            return query.InSingle(id);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="whereLambda">查询表达式</param> 
        /// <returns></returns>
        public bool IsExist(Expression<Func<T, bool>> whereLambda)
        {
            var datas = DbContext.Queryable<T>().Any(whereLambda);
            return datas;
        }
        /// <summary>
        /// 查询列最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public TResult GetMax<TResult>(Expression<Func<T, TResult>> expression)
        {
            return DbContext.Queryable<T>().Max(expression);
        }
        #endregion

        #region 分页查询

        /// <summary>
        /// 获取分页列表【页码，每页条数】
        /// </summary>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList(int pageIndex, int pageSize)
        {
            int count = 0;
            var query = DbContext.Queryable<T>();
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【排序，页码，每页条数】
        /// </summary>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList(Expression<Func<T, object>> orderExp, OrderByType orderType, int pageIndex, int pageSize)
        {
            int count = 0;
            var query = DbContext.Queryable<T>().OrderBy(orderExp, orderType);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【Linq表达式条件，页码，每页条数】
        /// </summary>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList(Expression<Func<T, bool>> whereExp, int pageIndex, int pageSize)
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(whereExp);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【Linq表达式条件，排序，页码，每页条数】
        /// </summary>
        /// <param name="whereExp">Linq表达式条件</param>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList(Expression<Func<T, bool>> whereExp, Expression<Func<T, object>> orderExp, OrderByType orderType, int pageIndex, int pageSize)
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(whereExp).OrderBy(orderExp, orderType);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        /// 获取分页列表【Sugar表达式条件，页码，每页条数】
        /// </summary>
        /// <param name="conditionals">Sugar条件表达式集合</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList(List<IConditionalModel> conditionals, int pageIndex, int pageSize)
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(conditionals);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }

        /// <summary>
        ///  获取分页列表【Sugar表达式条件，排序，页码，每页条数】
        /// </summary>
        /// <param name="conditionals">Sugar条件表达式集合</param>
        /// <param name="orderExp">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">页码（从0开始）</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>实体</returns>
        public PagedList<T> GetPageList(List<IConditionalModel> conditionals, Expression<Func<T, object>> orderExp, OrderByType orderType, int pageIndex, int pageSize)
        {
            int count = 0;
            var query = DbContext.Queryable<T>().Where(conditionals).OrderBy(orderExp, orderType);
            var result = query.ToPageList(pageIndex, pageSize, ref count);
            return new PagedList<T>(count, pageSize, result);
        }
        #endregion
    }

    #endregion
}
