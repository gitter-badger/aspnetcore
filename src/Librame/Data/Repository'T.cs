// Copyright (c) Librame.NET All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Librame.Data
{
    /// <summary>
    /// ʵ��ֿ⡣
    /// </summary>
    /// <typeparam name="T">ָ����ʵ�����͡�</typeparam>
    /// <author>Librame Pang</author>
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// ����һ��ʵ��ֿ�ʵ����
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// accessor Ϊ�ա�
        /// </exception>
        /// <param name="accessor">�����ķ�������</param>
        public Repository(IAccessor accessor)
        {
            if (ReferenceEquals(accessor, null))
                throw new ArgumentNullException("accessor");

            Accessor = accessor;
        }


        /// <summary>
        /// ��ȡ�����÷�������
        /// </summary>
        public virtual IAccessor Accessor { get; set; }

        /// <summary>
        /// ��ȡ��ǰʵ���ѯ����
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                // NHibernate.Linq.LinqExtensionMethods.Cacheable()
                return Accessor.Query<T>();
            }
        }


        /// <summary>
        /// ���ӵ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// �Ѽ��ɱ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���ӵ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        public virtual T Add(T entity)
        {
            return Accessor.Add(entity);
        }
        /// <summary>
        /// �첽���ӵ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// �Ѽ��ɱ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���ӵ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        public virtual async Task<T> AddAsync(T entity)
        {
            return await Accessor.AddAsync(entity);
        }

        /// <summary>
        /// ���µ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// �����е��ñ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        public virtual T Update(T entity)
        {
            return Accessor.Update(entity);
        }
        /// <summary>
        /// �첽���µ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// �����е��ñ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            return await Accessor.UpdateAsync(entity);
        }

        /// <summary>
        /// ���浥��ʵ�塣
        /// </summary>
        /// <remarks>
        /// ֧�����ӻ���²������Ѽ��ɱ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        public virtual T Save(T entity)
        {
            return Accessor.Save(entity);
        }
        /// <summary>
        /// �첽���浥��ʵ�塣
        /// </summary>
        /// <remarks>
        /// ֧�����ӻ���²������Ѽ��ɱ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        public virtual async Task<T> SaveAsync(T entity)
        {
            return await Accessor.SaveAsync(entity);
        }

        /// <summary>
        /// ɾ������ʵ�塣
        /// </summary>
        /// <remarks>
        /// �����е��ñ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        public virtual T Delete(T entity)
        {
            return Accessor.Delete(entity);
        }
        /// <summary>
        /// �첽ɾ������ʵ�塣
        /// </summary>
        /// <remarks>
        /// �����е��ñ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <typeparam name="T">ָ����ʵ�����͡�</typeparam>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        public virtual async Task<T> DeleteAsync(T entity)
        {
            return await Accessor.DeleteAsync(entity);
        }

        /// <summary>
        /// ���ʵ��仯�����浽���ݿ⡣
        /// </summary>
        /// <returns>������Ӱ���������</returns>
        public virtual int Flush()
        {
            return Accessor.Flush();
        }
        /// <summary>
        /// �첽���ʵ��仯�����浽���ݿ⡣
        /// </summary>
        /// <returns>������Ӱ���������</returns>
        public virtual async Task<int> FlushAsync()
        {
            return await Accessor.FlushAsync();
        }

        /// <summary>
        /// ������ѯͳ�ơ�
        /// </summary>
        /// <param name="predicate">�����Ĳ�ѯ�϶����ʽ��</param>
        /// <returns>����������</returns>
        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return Table.Where(predicate).Count();
        }
        /// <summary>
        /// �첽������ѯͳ�ơ�
        /// </summary>
        /// <param name="predicate">�����Ĳ�ѯ�϶����ʽ��</param>
        /// <returns>����������</returns>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.Where(predicate).CountAsync();
        }

        /// <summary>
        /// ��ȡƥ��ĵ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// ����ж��ƥ��ʵ�壬��Ĭ�Ϸ��ص�һ�
        /// </remarks>
        /// <typeparam name="T">ָ����ʵ�����͡�</typeparam>
        /// <param name="predicate">�϶�ʵ��ı��ʽ��</param>
        /// <returns>����ʵ�塣</returns>
        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            // �� Create and Update ��֤
            if (ReferenceEquals(predicate, null))
            {
                return null;
            }

            return Accessor.Get<T>(predicate);
        }
        /// <summary>
        /// �첽��ȡƥ��ĵ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// ����ж��ƥ��ʵ�壬��Ĭ�Ϸ��ص�һ�
        /// </remarks>
        /// <typeparam name="T">ָ����ʵ�����͡�</typeparam>
        /// <param name="predicate">�϶�ʵ��ı��ʽ��</param>
        /// <returns>����ʵ�塣</returns>
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            // �� Create and Update ��֤
            if (ReferenceEquals(predicate, null))
            {
                return null;
            }

            return await Accessor.GetAsync<T>(predicate);
        }

        /// <summary>
        /// �б��ѯ��
        /// </summary>
        /// <param name="filterFactory">�����Ĺ��˷�������ѡ����</param>
        /// <param name="sorterFactory">���������򷽷�����ѡ����</param>
        /// <returns>����ʵ���б�</returns>
        public virtual IList<T> List(Func<IQueryable<T>, IQueryable<T>> filterFactory = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory = null)
        {
            int total;
            return Table.Query(out total, filterFactory, sorterFactory);
        }
        /// <summary>
        /// �첽�б��ѯ��
        /// </summary>
        /// <param name="filterFactory">�����Ĺ��˷�����</param>
        /// <param name="sorterFactory">���������򷽷���</param>
        /// <returns>����ʵ���б�</returns>
        public virtual async Task<IList<T>> ListAsync(Func<IQueryable<T>, IQueryable<T>> filterFactory = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory = null)
        {
            return await Table.QueryAsync(filterFactory, sorterFactory);
        }

        /// <summary>
        /// ��ҳ�б��ѯ��
        /// </summary>
        /// <param name="filterFactory">�����Ĺ��˷�����</param>
        /// <param name="sorterFactory">���������򷽷���</param>
        /// <param name="skip">����������������</param>
        /// <param name="take">����ȡ�õ�������</param>
        /// <returns>���ط�ҳʵ���б�</returns>
        public virtual IPageable<T> PagedList(int skip, int take,
            Func<IQueryable<T>, IQueryable<T>> filterFactory = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory = null)
        {
            int total;
            return Table.Query(out total, filterFactory, sorterFactory, skip, take).ToPagedList(total, skip, take);
        }
        /// <summary>
        /// �첽��ҳ�б��ѯ��
        /// </summary>
        /// <param name="filterFactory">�����Ĺ��˷�����</param>
        /// <param name="sorterFactory">���������򷽷���</param>
        /// <param name="skip">����������������</param>
        /// <param name="take">����ȡ�õ�������</param>
        /// <returns>���ط�ҳʵ���б�</returns>
        public virtual async Task<IPageable<T>> PagedListAsync(int skip, int take,
            Func<IQueryable<T>, IQueryable<T>> filterFactory = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory = null)
        {
            // ���첽������֧�� OUT ����������ֻ�ֲܷ�
            int total = await Table.QueryTotalAsync(filterFactory, sorterFactory);
            var rows = await Table.QueryAsync(filterFactory, sorterFactory, skip, take);

            return rows.ToPagedList(total, skip, take);
        }


        #region IRepository<T> Members

        IAccessor IRepository<T>.Accessor
        {
            get { return Accessor; }
            set { Accessor = value; }
        }

        IQueryable<T> IRepository<T>.Table
        {
            get { return Table; }
        }

        T IRepository<T>.Add(T entity)
        {
            return Add(entity);
        }
        async Task<T> IRepository<T>.AddAsync(T entity)
        {
            return await AddAsync(entity);
        }

        T IRepository<T>.Update(T entity)
        {
            return Update(entity);
        }
        async Task<T> IRepository<T>.UpdateAsync(T entity)
        {
            return await UpdateAsync(entity);
        }

        T IRepository<T>.Save(T entity)
        {
            return Save(entity);
        }
        async Task<T> IRepository<T>.SaveAsync(T entity)
        {
            return await SaveAsync(entity);
        }

        T IRepository<T>.Delete(T entity)
        {
            return Delete(entity);
        }
        async Task<T> IRepository<T>.DeleteAsync(T entity)
        {
            return await DeleteAsync(entity);
        }

        int IRepository<T>.Flush()
        {
            return Flush();
        }
        async Task<int> IRepository<T>.FlushAsync()
        {
            return await FlushAsync();
        }

        int IRepository<T>.Count(Expression<Func<T, bool>> predicate)
        {
            return Count(predicate);
        }
        async Task<int> IRepository<T>.CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await CountAsync(predicate);
        }

        T IRepository<T>.Get(Expression<Func<T, bool>> predicate)
        {
            return Get(predicate);
        }
        async Task<T> IRepository<T>.GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetAsync(predicate);
        }

        IList<T> IRepository<T>.List(Func<IQueryable<T>, IQueryable<T>> filterFactory,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory)
        {
            return List(filterFactory, sorterFactory);
        }
        async Task<IList<T>> IRepository<T>.ListAsync(Func<IQueryable<T>, IQueryable<T>> filterFactory,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory)
        {
            return await ListAsync(filterFactory, sorterFactory);
        }

        IPageable<T> IRepository<T>.PagedList(int skip, int take,
            Func<IQueryable<T>, IQueryable<T>> filterFactory,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory)
        {
            return PagedList(skip, take, filterFactory, sorterFactory);
        }
        async Task<IPageable<T>> IRepository<T>.PagedListAsync(int skip, int take,
            Func<IQueryable<T>, IQueryable<T>> filterFactory,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory)
        {
            return await PagedListAsync(skip, take, filterFactory, sorterFactory);
        }

        #endregion


        /// <summary>
        /// �ͷŷ�������Դ��
        /// </summary>
        public virtual void Dispose()
        {
            if (!ReferenceEquals(Accessor, null))
            {
                Accessor.Dispose();
            }
        }

    }
}