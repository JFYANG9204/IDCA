﻿
namespace IDCA.Bll.MDMDocument
{
    public interface IField : IMDMLabeledObject, IMDMRange
    {
        /// <summary>
        /// 引用的对象，不引用返回null
        /// </summary>
        Field? Reference { get; }
        /// <summary>
        /// 当前对象的Category分类集合，不存在返回null
        /// </summary>
        Categories? Categories { get; }
        /// <summary>
        /// 所有的Element对象集合，脚本中由ElementType定义
        /// </summary>
        Elements? Elements { get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        MDMDataType DataType { get; }
        /// <summary>
        /// 下级变量集合，用于表示多级变量
        /// </summary>
        Variables? Items { get; }
    }

}
