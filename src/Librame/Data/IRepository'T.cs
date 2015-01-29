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
    /// ʵ��ֿ�ӿڡ�
    /// </summary>
    /// <typeparam name="T">ָ����ʵ�����͡�</typeparam>
    /// <author>Librame Pang</author>
    public interface IRepository<T> : IDisposable where T : class
    {
        /// <summary>
        /// ��ȡ�����÷�������
        /// </summary>
        IAccessor Accessor { get; set; }

        /// <summary>
        /// ��ȡ��ǰʵ���ѯ����
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// ���ӵ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// �Ѽ��ɱ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���ӵ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        T Add(T entity);
        /// <summary>
        /// �첽���ӵ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// �Ѽ��ɱ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���ӵ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// ���µ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// �����е��ñ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        T Update(T entity);
        /// <summary>
        /// �첽���µ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// �����е��ñ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// ���浥��ʵ�塣
        /// </summary>
        /// <remarks>
        /// ֧�����ӻ���²������Ѽ��ɱ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        T Save(T entity);
        /// <summary>
        /// �첽���浥��ʵ�塣
        /// </summary>
        /// <remarks>
        /// ֧�����ӻ���²������Ѽ��ɱ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        Task<T> SaveAsync(T entity);

        /// <summary>
        /// ɾ������ʵ�塣
        /// </summary>
        /// <remarks>
        /// �����е��ñ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        T Delete(T entity);
        /// <summary>
        /// �첽ɾ������ʵ�塣
        /// </summary>
        /// <remarks>
        /// �����е��ñ��浽���ݿⷽ�� <see cref="Flush()"/>��
        /// </remarks>
        /// <typeparam name="T">ָ����ʵ�����͡�</typeparam>
        /// <param name="entity">����Ҫ���µ�ʵ�塣</param>
        /// <returns>����ʵ�塣</returns>
        Task<T> DeleteAsync(T entity);

        /// <summary>
        /// ���ʵ��仯�����浽���ݿ⡣
        /// </summary>
        /// <returns>������Ӱ���������</returns>
        int Flush();
        /// <summary>
        /// �첽���ʵ��仯�����浽���ݿ⡣
        /// </summary>
        /// <returns>������Ӱ���������</returns>
        Task<int> FlushAsync();

        /// <summary>
        /// ������ѯͳ�ơ�
        /// </summary>
        /// <param name="predicate">�����Ĳ�ѯ�϶����ʽ��</param>
        /// <returns>����������</returns>
        int Count(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// �첽������ѯͳ�ơ�
        /// </summary>
        /// <param name="predicate">�����Ĳ�ѯ�϶����ʽ��</param>
        /// <returns>����������</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// ��ȡƥ��ĵ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// ����ж��ƥ��ʵ�壬��Ĭ�Ϸ��ص�һ�
        /// </remarks>
        /// <typeparam name="T">ָ����ʵ�����͡�</typeparam>
        /// <param name="predicate">�϶�ʵ��ı��ʽ��</param>
        /// <returns>����ʵ�塣</returns>
        T Get(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// �첽��ȡƥ��ĵ���ʵ�塣
        /// </summary>
        /// <remarks>
        /// ����ж��ƥ��ʵ�壬��Ĭ�Ϸ��ص�һ�
        /// </remarks>
        /// <typeparam name="T">ָ����ʵ�����͡�</typeparam>
        /// <param name="predicate">�϶�ʵ��ı��ʽ��</param>
        /// <returns>����ʵ�塣</returns>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// �б��ѯ��
        /// </summary>
        /// <param name="filterFactory">�����Ĺ��˷�������ѡ����</param>
        /// <param name="sorterFactory">���������򷽷�����ѡ����</param>
        /// <returns>����ʵ���б�</returns>
        IList<T> List(Func<IQueryable<T>, IQueryable<T>> filterFactory = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory = null);
        /// <summary>
        /// �첽�б��ѯ��
        /// </summary>
        /// <param name="filterFactory">�����Ĺ��˷�������ѡ����</param>
        /// <param name="sorterFactory">���������򷽷�����ѡ����</param>
        /// <returns>����ʵ���б�</returns>
        Task<IList<T>> ListAsync(Func<IQueryable<T>, IQueryable<T>> filterFactory = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory = null);

        /// <summary>
        /// ��ҳ�б��ѯ��
        /// </summary>
        /// <param name="skip">����������������</param>
        /// <param name="take">����ȡ�õ�������</param>
        /// <param name="filterFactory">�����Ĺ��˷�������ѡ����</param>
        /// <param name="sorterFactory">���������򷽷�����ѡ����</param>
        /// <returns>���ط�ҳʵ���б�</returns>
        IPageable<T> PagedList(int skip, int take,
            Func<IQueryable<T>, IQueryable<T>> filterFactory = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory = null);
        /// <summary>
        /// �첽��ҳ�б��ѯ��
        /// </summary>
        /// <param name="skip">����������������</param>
        /// <param name="take">����ȡ�õ�������</param>
        /// <param name="filterFactory">�����Ĺ��˷�������ѡ����</param>
        /// <param name="sorterFactory">���������򷽷�����ѡ����</param>
        /// <returns>���ط�ҳʵ���б�</returns>
        Task<IPageable<T>> PagedListAsync(int skip, int take,
            Func<IQueryable<T>, IQueryable<T>> filterFactory = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sorterFactory = null);
    }
}