// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Data;
// using System.Diagnostics;
// using System.IO;
// using System.Linq;
// using System.Reflection;
// using System.Runtime.Serialization.Formatters.Binary;
// using System.Text;
// using ExcelDataReader;
// using LitJson;
// using OfficeOpenXml;
// using UnityEditor;
// using UnityEditorInternal;
// using UnityEngine;
// using UnityEngine.Rendering;
// using Debug = UnityEngine.Debug;
// using Object = UnityEngine.Object;
//
// namespace EditorTools
// {
//     /// <summary>
//     /// 数据工具
//     /// </summary>
//     public class DataTools : EditorWindow
//     {
//         //[MenuItem(CommonEditorMethod.CommonTitle +"编辑Data#E #E")]
//         public static void BuildPackageVersions()
//         {
//             if (!HasOpenInstances<DataTools>())
//                 GetWindow(typeof(DataTools), false, "DataTools").Show();
//             else
//                 GetWindow(typeof(DataTools)).Close();
//         }
//
//
//         // private void OnEnable() => GetType().Load();
//         // private void OnDisable() => GetType().Save();
//
//         private void OnGUI()
//         {
//             GUI.backgroundColor = Color.yellow;
//             Title();
//             EditorGUILayout.BeginHorizontal();
//
//             //LeftUI();
//             //RightUI();
//             EditorGUILayout.EndHorizontal();
//         }
//
//         private void Title()
//         {
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button("测试按钮")) Debug(1);
//             if (GUILayout.Button("测试按钮")) Debug(1);
//             if (GUILayout.Button("测试按钮")) Debug(1);
//             EditorGUILayout.EndHorizontal();
//         }
//
//
//         private void BinarySaveData(string path)
//         {
//             //没有路径创建路径
//             if (!Directory.Exists(path))
//                 Directory.CreateDirectory(path);
//             //写入文件
//             using FileStream fileStream = new FileStream(path, FileMode.Create);
//             //去Byte文件写入数据
//             using var binaryWriter = new BinaryWriter(fileStream);
//         }
//
//
//         private void Debug(int o)
//         {
//             UnityEngine.Debug.Log(o);
//         }
//     }
//
//     /// <summary>
//     /// 动画生成器
//     /// </summary>
//     public class AnimationCreat : EditorWindow
//     {
//         //[MenuItem(CommonEditorMethod.CommonTitle + "动画创建器#E #E")]
//         public static void BuildPackageVersions()
//         {
//             if (!HasOpenInstances<AnimationCreat>())
//                 GetWindow(typeof(AnimationCreat), false, "DataTools").Show();
//             else
//                 GetWindow(typeof(AnimationCreat)).Close();
//         }
//
//         #region 存读
//
//         private void OnEnable() => Load();
//         private void OnDisable() => Save();
//
//         private void Save()
//         {
//             if (!HasOpenInstances<AnimationCreat>()) return;
//             Type type = GetType();
//             var fieldsValue = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
//             foreach (var data in fieldsValue)
//             {
//                 if (data.FieldType == typeof(string))
//                     PlayerPrefs.SetString($"{Application.productName}{data.Name}Save", (string)data.GetValue(this));
//                 if (data.FieldType == typeof(int))
//                     PlayerPrefs.SetInt($"{Application.productName}{data.Name}Save", (int)data.GetValue(this));
//             }
//
//             Debug.Log("保存成功");
//         }
//
//         private void Load()
//         {
//             Type type = GetType();
//             var fieldsValue = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
//             foreach (var data in fieldsValue)
//             {
//                 if (data.FieldType == typeof(string))
//                     data.SetValue(this, PlayerPrefs.GetString($"{Application.productName}{data.Name}Save"));
//                 if (data.FieldType == typeof(int))
//                     data.SetValue(this, PlayerPrefs.GetInt($"{Application.productName}{data.Name}Save"));
//             }
//         }
//
//         #endregion
//
//         private void OnGUI()
//         {
//             GUI.backgroundColor = Color.yellow;
//             Title();
//             EditorGUILayout.BeginHorizontal();
//
//             //LeftUI();
//             //RightUI();
//             EditorGUILayout.EndHorizontal();
//         }
//
//         private void Title()
//         {
//             EditorGUILayout.BeginHorizontal();
//             // if (GUILayout.Button("测试按钮")) Debug(1);
//             // if (GUILayout.Button("测试按钮")) Debug(1);
//             // if (GUILayout.Button("测试按钮")) Debug(1);
//             EditorGUILayout.EndHorizontal();
//         }
//     }
//
//     /// <summary>
//     /// Excel工具
//     /// </summary>
//     public class ExcelTools : EditorWindow
//     {
//         private Vector2 _scroll1, _scroll2 = Vector2.zero; //滑动
//         public static string[][] Data; //数据
//
//         private string _openFolderPath; //加载的文件夹路径
//         private string _loadExcelPath; //加载的文件路径
//
//         private GUIStyle _centeredStyle; //居中
//         private GUIStyle _styleRight; //居右边
//         private GUIStyle _styleLight; //居左边
//
//         private string _message; //消息
//
//         private string _binaryPath;
//         private string _cSharpPath;
//         private string _save3Path;
//         private string _saveJsonPath;
//         private string _saveAssetPath;
//
//         private static string binaryDataSavePath = string.Empty;
//
//
//         [MenuItem(CommonEditorMethod.CommonTitle + "编辑Excel")]
//         public static void ShowUI() => "编辑Excel".ShowUI<ExcelTools>();
//
//         private void Awake()
//         {
//             _centeredStyle ??= new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
//             _styleRight ??= new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleRight };
//             _styleLight ??= new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleLeft };
//         }
//
//         private void OnEnable()
//         {
//             this.Load();
//             Data = _loadExcelPath.ReadExcel(0);
//         }
//
//         private void OnDisable()
//         {
//             this.Save();
//         }
//
//         private void OnGUI()
//         {
//             GUI.backgroundColor = Color.yellow;
//             EditorGUILayout.BeginHorizontal();
//             LeftUI();
//             RightUI();
//             EditorGUILayout.EndHorizontal();
//             Repaint(); //实时刷新
//         }
//
//
//         private void LeftUI()
//         {
//             float width = 150f;
//             float height = Screen.height;
//
//             EditorGUILayout.BeginVertical(GUILayout.Width(width));
//             if (GUILayout.Button("选择文件夹路径...", GUILayout.Width(width))) _openFolderPath = EditorUtility.OpenFolderPanel("选择文件夹", default, default); //打开文件夹
//             if (GUILayout.Button("打开文件夹", GUILayout.Width(width))) Process.Start(_openFolderPath);
//             EditorGUILayout.LabelField("文件夹路径:", EditorStyles.label, GUILayout.Width(80));
//             EditorGUILayout.TextField(_openFolderPath);
//
//
//             if (string.IsNullOrEmpty(_openFolderPath)) return;
//             if (!Directory.Exists(_openFolderPath)) _openFolderPath = string.Empty;
//
//             string[] files = Directory.GetFiles(_openFolderPath, "*.xlsx", SearchOption.AllDirectories);
//             foreach (var path in files)
//             {
//                 string fileName = Path.GetFileName(path);
//                 if (GUILayout.Button(fileName, GUILayout.Width(width))) Data = path.ReadExcel(0);
//             }
//
//             EditorGUILayout.EndVertical();
//         }
//
//         private void RightUI()
//         {
//             GUILayout.Space(10f);
//             EditorGUILayout.BeginVertical();
//             {
//                 EditorGUILayout.BeginHorizontal();
//                 EditorGUILayout.LabelField("消息提示：", EditorStyles.boldLabel, GUILayout.Width(60f));
//                 EditorGUILayout.LabelField(_message, GUILayout.Width(200f));
//                 EditorGUILayout.LabelField("其他功能:", _centeredStyle, GUILayout.Width(100f));
//                 if (GUILayout.Button("清空Unity日志", GUILayout.Width(100f))) ClearConsole();
//                 if (GUILayout.Button("创建模板Excel", GUILayout.Width(100f))) CreatPsdExcel();
//                 EditorGUILayout.EndHorizontal();
//
//                 //二进制文件路径
//                 EditorGUILayout.BeginHorizontal();
//                 if (GUILayout.Button("打开二进制面板", GUILayout.Width(100f))) BinaryTools.ShowUI();
//                 if (GUILayout.Button("打开Json面板", GUILayout.Width(100f))) JsonTools.ShowUI();
//                 if (GUILayout.Button("打开Asset面板", GUILayout.Width(100f))) AssetTools.ShowUI();
//                 if (GUILayout.Button("打开C#面板", GUILayout.Width(100f))) CSharpTools.ShowUI();
//                 EditorGUILayout.EndHorizontal();
//
//                 if (!string.IsNullOrEmpty(_loadExcelPath))
//                 {
//                     RefreshExcelTitleData();
//                     RefreshExcelData();
//                 }
//             }
//             EditorGUILayout.EndVertical();
//         }
//
//         /// <summary>
//         /// 刷新Exce按钮数据
//         /// </summary>
//         private void RefreshExcelTitleData()
//         {
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button("添加数据", GUILayout.Width(80f)))
//             {
//                 string[] dataArray = new string[Data[0].Length];
//                 List<string[]> strings = Data.ToList();
//                 strings.Add(dataArray);
//                 Data = strings.ToArray();
//             }
//
//             if (GUILayout.Button("写入数据", GUILayout.Width(80f)))
//             {
//                 FileInfo excelName = new FileInfo(_loadExcelPath);
//                 using ExcelPackage package = new ExcelPackage(excelName); //通过ExcelPackage打开文件
//                 ExcelWorksheet worksheet = package.Workbook.Worksheets[1]; //1表示第一个表//Debug.Log(worksheet.Name);
//                 for (int i = 0; i < Data.Length; i++)
//                 {
//                     string[] item1 = Data[i];
//                     for (int j = 0; j < item1.Length; j++)
//                     {
//                         string item2 = item1[j];
//                         if (int.TryParse(item2, out int number1))
//                             worksheet.SetValue(i + 1, j + 1, number1);
//                         else if (float.TryParse(item2, out float number2))
//                             worksheet.SetValue(i + 1, j + 1, number2);
//                         else if (string.IsNullOrEmpty(item2))
//                             worksheet.SetValue(i + 1, j + 1, null);
//                         else
//                             worksheet.Cells[i + 1, j + 1].Value = item2;
//                     }
//                 }
//
//                 package.Save(); //储存
//                 Message("写入成功");
//             }
//
//             if (GUILayout.Button("Excel转换", GUILayout.Width(80f)))
//                 GenerateExcelInfo(_loadExcelPath);
//             if (GUILayout.Button("打开Excel", GUILayout.Width(80f)))
//                 Application.OpenURL(_loadExcelPath);
//             EditorGUILayout.EndHorizontal();
//         }
//
//         /// <summary>
//         /// 刷新Excel数据
//         /// </summary>
//         private void RefreshExcelData()
//         {
//             if (Data == null || Data.Length == 0) return;
//             float width = position.width - 180;
//
//             //显示数据
//             float height = position.height - 200f;
//             _scroll2 = GUILayout.BeginScrollView(_scroll2, true, false, GUILayout.Width(width), GUILayout.Height(height));
//             for (int i = (int)RowType.BeginIndex; i < Data.Length; i++)
//             {
//                 string[] item1 = Data[i];
//                 EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width));
//                 if (GUILayout.Button("删除", GUILayout.Width(45)))
//                 {
//                     //删除内存数据
//                     List<string[]> strings = new List<string[]>(Data);
//                     strings.Remove(item1);
//                     Data = strings.ToArray();
//                     //删除实际数据
//                     FileInfo excelName = new FileInfo(_loadExcelPath);
//                     //通过ExcelPackage打开文件
//                     using var package = new ExcelPackage(excelName);
//                     ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
//                     for (var j = 0; j < Data[0].Length; j++)
//                         worksheet.SetValue(i + 1, j + 1, null);
//                     package.Save(); //储存
//                 }
//
//                 for (var j = 0; j < item1.Length; j++)
//                     Data[i][j] = EditorGUILayout.TextField(item1[j], GUILayout.MinWidth(100));
//
//                 EditorGUILayout.EndHorizontal();
//             }
//
//             GUILayout.EndScrollView();
//         }
//
//         /// <summary>
//         /// 生成文件
//         /// </summary>
//         /// <param name="path"></param>
//         private static void GenerateExcelInfo(string path)
//         {
//             IEnumerable<string> paths = Directory.EnumerateFiles(path, "*.xlsx");
//             foreach (var filePath in paths)
//             {
//                 string[][] data = filePath.ReadExcel(0); //读取Excel数据
//                 //CreateScript(filePath, data); //生成C#文件
//                 CreateByte(filePath, data); //生成二进制文件
//             }
//
//             AssetDatabase.Refresh(); //刷新Project窗口
//         }
//
//
//         /// <summary>
//         /// 创建二进制文件
//         /// </summary>
//         private static void CreateByte(string filePath, string[][] data)
//         {
//             string className = new FileInfo(filePath).Name.Split('.')[0];
//
//             if (!Directory.Exists(binaryDataSavePath))
//                 Directory.CreateDirectory(binaryDataSavePath);
//             AssetDatabase.Refresh();
//             var path = $"{binaryDataSavePath}/{className}.bytes";
//             //写入文件
//             using var fileStream = new FileStream(path, FileMode.Create);
//             //创建类型
//             List<Type> types = GetTypeByFieldType(data);
//             //去Byte文件写入数据
//             using var binaryWriter = new BinaryWriter(fileStream);
//             for (var i = (int)RowType.BeginIndex; i < data.Length; ++i) //开始读取真实数据
//             {
//                 for (var j = 0; j < types.Count; ++j)
//                 {
//                     //获取数据的bytes
//                     var typeTemp = types[j];
//                     var dataTemp = data[i][j];
//                     if (string.IsNullOrEmpty(dataTemp)) //跳过空的数据
//                         continue;
//                     var bytes = GetBasicField(typeTemp, dataTemp);
//                     //写入数据
//                     binaryWriter.Write(bytes);
//                 }
//             }
//         }
//
//         /// <summary>
//         /// 获取字节
//         /// </summary>
//         /// <param name="type">例如List<Int>的数据在表格也当成string类型看</param>
//         /// <param name="data"></param>
//         /// <returns></returns>
//         /// <exception cref="Exception"></exception>
//         private static byte[] GetBasicField(Type type, string data)
//         {
//             byte[] bytes = null;
//             if (type == typeof(int))
//                 bytes = BitConverter.GetBytes(int.Parse(data));
//             else if (type == typeof(float))
//                 bytes = BitConverter.GetBytes(float.Parse(data));
//             else if (type == typeof(bool))
//                 bytes = BitConverter.GetBytes(bool.Parse(data));
//             else if (type == typeof(string) ||
//                      type == typeof(List<string>) ||
//                      type == typeof(List<int>) ||
//                      type == typeof(List<float>)
//                      //TODO自己定义的类型
//                     )
//             {
//                 byte[] dataBytes = Encoding.Default.GetBytes(data);
//                 List<byte> lengthBytes = BitConverter.GetBytes(dataBytes.Length).ToList();
//                 lengthBytes.AddRange(dataBytes);
//                 bytes = lengthBytes.ToArray();
//             }
//
//             if (bytes == null) throw new Exception($"{nameof(UnityEngine.Object.name)}.GetBasicField: 其类型未配置或不是基础类型 Type:{type} Data:{data}");
//             return bytes;
//         }
//
//         private static List<Type> GetTypeByFieldType(string[][] data)
//         {
//             List<Type> types = new List<Type>();
//             string[] temp = data[(int)RowType.FieldIndex]; //获取类型
//             foreach (var t in temp)
//             {
//                 if (t == "int") types.Add(typeof(int));
//                 else if (t == "bool") types.Add(typeof(bool));
//                 else if (t == "float") types.Add(typeof(float));
//                 else if (t == "string") types.Add(typeof(string));
//                 else if (t == "List<int>") types.Add(typeof(List<int>));
//                 else if (t == "List<string>") types.Add(typeof(List<string>));
//                 else if (t == "List<float>") types.Add(typeof(List<float>));
//             }
//
//             return types;
//         }
//
//
//         /// <summary>
//         /// 创建excel模板文件
//         /// </summary>
//         private void CreatPsdExcel()
//         {
//             FileInfo newFile = new FileInfo($@"{_openFolderPath}\NewExcelFile.xlsx"); // 创建一个新的 Excel 文件
//             using ExcelPackage package = new ExcelPackage(newFile);
//             ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1"); // 添加一个工作表
//             worksheet.Cells["A1"].Value = "itemID"; // 写入数据
//             worksheet.Cells["A2"].Value = "int"; // 写入数据
//             worksheet.Cells["A3"].Value = "物品id"; // 写入数据
//             worksheet.Cells["A4"].Value = 1; // 写入数据
//             package.Save(); // 保存文件
//         }
//
//         /// <summary>
//         /// 清空日志
//         /// </summary>
//         private static void ClearConsole()
//         {
//             var assembly = Assembly.GetAssembly(typeof(SceneView));
//             var logEntries = assembly.GetType("UnityEditor.LogEntries");
//             var clearConsoleMethod = logEntries.GetMethod("Clear");
//             clearConsoleMethod?.Invoke(new object(), null);
//         }
//
//
//         private void Message(string content, bool opException = false)
//         {
//             _message = content;
//             if (opException)
//                 throw new Exception(content);
//         }
//     }
//
//     public class AssetTools : EditorWindow
//     {
//         private string _assetName;
//         private string _saveCSharpPath;
//         private string _cSharpName;
//         private string _saveAssetPath;
//
//         private void OnEnable() => this.Load();
//         private void OnDisable() => this.Save();
//         public static void ShowUI() => "Asset".ShowUI<AssetTools>();
//
//         void OnGUI()
//         {
//             GUI.backgroundColor = Color.yellow;
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("C#:", GUILayout.Width(100f));
//             _assetName = EditorGUILayout.TextField(_cSharpName);
//
//             EditorGUILayout.LabelField("Asset名称:", GUILayout.Width(100f));
//             _assetName = EditorGUILayout.TextField(_assetName);
//             EditorGUILayout.EndHorizontal();
//
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button("选择存储路径", GUILayout.Width(70f))) _saveAssetPath = EditorUtility.OpenFolderPanel("选择文件夹", default, default);
//             if (GUILayout.Button("打开Asset文件路径", GUILayout.Width(100f))) Process.Start(_saveAssetPath);
//             if (GUILayout.Button("生成", GUILayout.Width(100f))) ChangeAsset(_saveAssetPath);
//             _saveAssetPath = EditorGUILayout.TextField(_saveAssetPath);
//             EditorGUILayout.EndHorizontal();
//
//             Repaint(); //实时刷新
//         }
//
//         private void ChangeAsset(string saveCSharpPath)
//         {
//             CreateScript(saveCSharpPath);
//             CreatScriptableObject(saveCSharpPath);
//
//             string[][] data = ExcelTools.Data;
//             string sonName = data[(int)RowType.ConfigIndex][(int)ColType.ScriptableObjectName];
//             sonName = sonName.Split(':')[1];
//             string className = data[(int)RowType.ConfigIndex][(int)ColType.ClassName];
//             className = className.Split(':')[1];
//             // 通过字符串获取ScriptableObject类型
//             Assembly assembly = Assembly.Load("Assembly-CSharp"); // 加载指定程序集
//             if (assembly == null) throw new Exception("加载程序集失败Assembly-CSharp");
//             Type scriptableObjectType = assembly.GetType(sonName); // 通过字符串获取ScriptableObject类型
//             if (scriptableObjectType == null)
//                 scriptableObjectType = Type.GetType(sonName); //本程序集
//
//             //Type scriptableObjectType = Type.GetType(className);
//             if (scriptableObjectType == null || !scriptableObjectType.IsSubclassOf(typeof(ScriptableObject)))
//                 throw new Exception("无效的ScriptableObject类型");
//             // 创建ScriptableObject实例
//             ScriptableObject scriptableObject = ScriptableObject.CreateInstance(scriptableObjectType);
//             
//             Type listType = typeof(List<>); // 获取未指定类型的List类型
//             Type elementType = assembly.GetType(className); // 指定List的元素类型，
//             //Type elementType = typeof(string); // 指定List的元素类型，这里假设为string
//             Type concreteListType = listType.MakeGenericType(elementType); // 创建具体类型的List
//             IList list = (IList)Activator.CreateInstance(concreteListType); // 实例化List对象
//             
//             string[] d11 = data[(int)RowType.FieldIndex]; //字段
//             for (int i = (int)RowType.DataIndex; i < data.Length; i++) //设置数据
//             {
//                 string[] d1 = data[i]; //数据
//
//                 object o = Activator.CreateInstance(elementType); //创建类
//                 for (int j = 0; j < d11.Length; j++)
//                 {
//                     string field = d11[j]; //字段
//                     string value = d1[j]; //值
//                     FieldInfo fieldInfo = o.GetType().GetField(field);
//                     if (fieldInfo.FieldType == typeof(string))
//                         fieldInfo.SetValue(o, value);
//                     else if (fieldInfo.FieldType == typeof(int))
//                         fieldInfo.SetValue(o, int.Parse(value));
//                 }
//
//                 list.Add(o);
//             }
//
//             FieldInfo[] fields = scriptableObjectType.GetFields();
//             foreach (FieldInfo fieldInfo in fields)
//             {
//                 if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
//                 {
//                     fieldInfo.SetValue(scriptableObject, list);
//                     // fieldInfo.SetValue(scriptableObject,t3);
//                     // foreach (var o in t3)
//                     // {
//                     //     MethodInfo addMethod = fieldInfo.FieldType.GetMethod("Add"); // 获取List的Add方法
//                     //     addMethod?.Invoke(scriptableObject, new object[] { o }); // 调用Add方法向List中添加数据
//                     // }
//                 }
//             }
//
//
//             // 保存ScriptableObject到Assets文件夹
//             //把当前实例储存为.asset资源文件，当作技能配置
//             //string assetPath = saveCSharpPath + "/" + className + ".asset";
//             //如果资源对象已存在，先进行删除，在进行创建
//             string assetPath = "Assets/NewScriptableObject.asset";
//             AssetDatabase.DeleteAsset(assetPath);
//             AssetDatabase.CreateAsset(scriptableObject, assetPath);
//             AssetDatabase.SaveAssets();
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <param name="type">实力类</param>
//         /// <param name="fieldInfo"></param>
//         /// <param name="s"></param>
//         public void SetValue(FieldInfo fieldInfo, string s)
//         {
//             if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
//             {
//                 // MethodInfo addMethod = fieldInfo.FieldType.GetMethod("Add"); // 获取List的Add方法
//                 // addMethod?.Invoke(fieldInfo, new object[] { o }); // 调用Add方法向List中添加数据
//             }
//             else if (fieldInfo.FieldType == typeof(string))
//             {
//                 fieldInfo.SetValue(fieldInfo, s);
//             }
//             else if (fieldInfo.FieldType == typeof(int))
//             {
//                 fieldInfo.SetValue(fieldInfo, int.Parse(s));
//             }
//         }
//
//
//         private void CreatScriptableObject(string saveCSharpPath)
//         {
//             if (string.IsNullOrEmpty(saveCSharpPath)) throw new Exception("没有路径");
//
//             string[][] data = ExcelTools.Data;
//             StringBuilder sb = new StringBuilder();
//             string sonName = data[(int)RowType.ConfigIndex][(int)ColType.ScriptableObjectName];
//             sonName = sonName.Split(':')[1];
//             string path = $"{saveCSharpPath}/{sonName}.cs";
//
//             sb.AppendLine("using System.Collections.Generic;");
//             sb.AppendLine("using UnityEngine;");
//             sb.AppendLine("using System;");
//             sb.AppendLine();
//             sb.AppendLine("[CreateAssetMenu(fileName = \"NewScriptableObject\", menuName = \"Tools/创建{className}\", order = 51)]");
//             sb.AppendLine($"public class {sonName} : ScriptableObject");
//             sb.AppendLine("{");
//             string className = data[(int)RowType.ConfigIndex][(int)ColType.ClassName];
//             className = className.Split(':')[1];
//             sb.AppendLine($"\tpublic List<{className}> {className}List;");
//             sb.AppendLine("}");
//
//             File.Delete(path);
//             File.WriteAllText(path, sb.ToString());
//             AssetDatabase.Refresh();
//         }
//
//         /// <summary>
//         /// 通过Excel数据生成脚本文件
//         /// </summary>
//         private static void CreateScript(string filePath)
//         {
//             string[][] data = ExcelTools.Data;
//             StringBuilder sb = new StringBuilder();
//             string className = data[(int)RowType.ConfigIndex][(int)ColType.ClassName];
//             className = className.Split(':')[1];
//             string path = $"{filePath}/{className}.cs";
//
//             sb.AppendLine("using System.Collections.Generic;");
//             sb.AppendLine("using System;");
//             sb.AppendLine();
//             sb.AppendLine($"/// <summary>");
//             sb.AppendLine($"/// {data[(int)RowType.ConfigIndex][(int)ColType.Chinese]}");
//             sb.AppendLine($"/// </summary>");
//             sb.AppendLine($"[Serializable]");
//             sb.AppendLine($"public class {className}");
//             sb.AppendLine("{");
//             int count = data[(int)RowType.TypeIndex].Length;
//             for (int i = 0; i < count; i++)
//             {
//                 string v1 = data[(int)RowType.TypeIndex][i];
//                 string v2 = data[(int)RowType.FieldIndex][i];
//                 string v3 = data[(int)RowType.DesIndex][i];
//
//                 sb.AppendLine($"\t/// <summary>");
//                 sb.AppendLine($"\t/// {v3}");
//                 sb.AppendLine($"\t/// </summary>");
//                 sb.AppendLine($"\tpublic {v1} {v2};");
//             }
//
//             sb.AppendLine("}");
//             File.Delete(path);
//             File.WriteAllText(path, sb.ToString());
//             AssetDatabase.Refresh();
//         }
//
//         private static List<Type> GetTypeByFieldType(string[][] data)
//         {
//             List<Type> types = new List<Type>();
//             string[] temp = data[(int)RowType.TypeIndex]; //获取类型
//             foreach (string t in temp)
//             {
//                 if (t == "int") types.Add(typeof(int));
//                 else if (t == "bool") types.Add(typeof(bool));
//                 else if (t == "float") types.Add(typeof(float));
//                 else if (t == "string") types.Add(typeof(string));
//                 else if (t == "List<int>") types.Add(typeof(List<int>));
//                 else if (t == "List<string>") types.Add(typeof(List<string>));
//                 else if (t == "List<float>") types.Add(typeof(List<float>));
//             }
//
//             return types;
//         }
//     }
//
//     public class BinaryTools : EditorWindow
//     {
//         private string _assetName;
//         private string _saveCSharpPath;
//         private string _cSharpName;
//         private string _saveAssetPath;
//
//         private void OnEnable() => this.Load();
//         private void OnDisable() => this.Save();
//         public static void ShowUI() => "Binary".ShowUI<AssetTools>();
//
//         void OnGUI()
//         {
//             GUI.backgroundColor = Color.yellow;
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("C#:", GUILayout.Width(100f));
//             _assetName = EditorGUILayout.TextField(_cSharpName);
//
//             EditorGUILayout.LabelField("Asset名称:", GUILayout.Width(100f));
//             _assetName = EditorGUILayout.TextField(_assetName);
//             EditorGUILayout.EndHorizontal();
//
//
//             //C#文件路径
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button("选择存储路径", GUILayout.Width(70f))) _saveCSharpPath = EditorUtility.OpenFolderPanel("选择文件夹", default, default);
//             if (GUILayout.Button("打开C#文件路径", GUILayout.Width(100f))) Process.Start(_saveCSharpPath);
//             if (GUILayout.Button("生成", GUILayout.Width(100f))) ChangeCSharp(_saveCSharpPath);
//             _saveCSharpPath = EditorGUILayout.TextField(_saveCSharpPath);
//             EditorGUILayout.EndHorizontal();
//
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button("选择存储路径", GUILayout.Width(70f))) _saveAssetPath = EditorUtility.OpenFolderPanel("选择文件夹", default, default);
//             if (GUILayout.Button("打开Asset文件路径", GUILayout.Width(100f))) Process.Start(_saveAssetPath);
//             if (GUILayout.Button("生成", GUILayout.Width(100f))) ChangeAsset(_saveAssetPath);
//             _saveAssetPath = EditorGUILayout.TextField(_saveAssetPath);
//             EditorGUILayout.EndHorizontal();
//
//             Repaint(); //实时刷新
//         }
//
//         private void ChangeAsset(string saveCSharpPath)
//         {
//             //生成C# ScriptableObject文件
//
//
//             //把当前实例储存为.asset资源文件，当作技能配置
//             // string assetPath = "Assets/Resources/Data/Asset" + skillCfg.skillid + ".asset";
//             // //如果资源对象已存在，先进行删除，在进行创建
//             // AssetDatabase.DeleteAsset(assetPath);
//             // AssetDatabase.CreateAsset(skillDataCfg, assetPath);
//         }
//
//         private void ChangeCSharp(string saveCSharpPath)
//         {
//             var data = ExcelTools.Data;
//         }
//     }
//
//     public class JsonTools : EditorWindow
//     {
//         private string _assetName;
//         private string _saveCSharpPath;
//         private string _cSharpName;
//         private string _saveAssetPath;
//
//         private void OnEnable() => this.Load();
//         private void OnDisable() => this.Save();
//         public static void ShowUI() => "Json".ShowUI<JsonTools>();
//
//         void OnGUI()
//         {
//             GUI.backgroundColor = Color.yellow;
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("C#:", GUILayout.Width(100f));
//             _assetName = EditorGUILayout.TextField(_cSharpName);
//
//             EditorGUILayout.LabelField("Asset名称:", GUILayout.Width(100f));
//             _assetName = EditorGUILayout.TextField(_assetName);
//             EditorGUILayout.EndHorizontal();
//
//
//             //C#文件路径
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button("选择存储路径", GUILayout.Width(70f))) _saveCSharpPath = EditorUtility.OpenFolderPanel("选择文件夹", default, default);
//             if (GUILayout.Button("打开C#文件路径", GUILayout.Width(100f))) Process.Start(_saveCSharpPath);
//             if (GUILayout.Button("生成", GUILayout.Width(100f))) ChangeCSharp(_saveCSharpPath);
//             _saveCSharpPath = EditorGUILayout.TextField(_saveCSharpPath);
//             EditorGUILayout.EndHorizontal();
//
//             // //C#文件路径
//             // EditorGUILayout.BeginHorizontal();
//             // if (GUILayout.Button("选择存储路径", GUILayout.Width(70f))) _saveJsonPath = EditorUtility.OpenFolderPanel("选择文件夹", default, default);
//             // if (GUILayout.Button("打开Json文件路径", GUILayout.Width(100f))) Process.Start(_saveJsonPath);
//             // if (GUILayout.Button("生成", GUILayout.Width(100f))) ChangeJson(_saveJsonPath, _loadExcelPath);
//             // _saveJsonPath = EditorGUILayout.TextField(_saveJsonPath);
//
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button("选择存储路径", GUILayout.Width(70f))) _saveAssetPath = EditorUtility.OpenFolderPanel("选择文件夹", default, default);
//             if (GUILayout.Button("打开Asset文件路径", GUILayout.Width(100f))) Process.Start(_saveAssetPath);
//             if (GUILayout.Button("生成", GUILayout.Width(100f))) ChangeAsset(_saveAssetPath);
//             _saveAssetPath = EditorGUILayout.TextField(_saveAssetPath);
//             EditorGUILayout.EndHorizontal();
//
//             Repaint(); //实时刷新
//         }
//
//         private void ChangeAsset(string saveCSharpPath)
//         {
//             //生成C# ScriptableObject文件
//
//
//             //把当前实例储存为.asset资源文件，当作技能配置
//             // string assetPath = "Assets/Resources/Data/Asset" + skillCfg.skillid + ".asset";
//             // //如果资源对象已存在，先进行删除，在进行创建
//             // AssetDatabase.DeleteAsset(assetPath);
//             // AssetDatabase.CreateAsset(skillDataCfg, assetPath);
//         }
//
//         private void ChangeCSharp(string saveCSharpPath)
//         {
//             var data = ExcelTools.Data;
//         }
//     }
//
//     public class CSharpTools : EditorWindow
//     {
//         private string _assetName;
//         private string _saveCSharpPath;
//         private string _cSharpName;
//         private string _saveAssetPath;
//
//         private void OnEnable() => this.Load();
//         private void OnDisable() => this.Save();
//         public static void ShowUI() => "CSharp".ShowUI<CSharpTools>();
//
//         void OnGUI()
//         {
//             GUI.backgroundColor = Color.yellow;
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField("C#:", GUILayout.Width(100f));
//             _assetName = EditorGUILayout.TextField(_cSharpName);
//
//             EditorGUILayout.LabelField("Asset名称:", GUILayout.Width(100f));
//             _assetName = EditorGUILayout.TextField(_assetName);
//             EditorGUILayout.EndHorizontal();
//
//
//             // //C#文件路径
//             // EditorGUILayout.BeginHorizontal();
//             // if (GUILayout.Button("选择存储路径", GUILayout.Width(70f))) _saveCSharpPath = EditorUtility.OpenFolderPanel("选择文件夹", default, default);
//             // if (GUILayout.Button("打开C#文件路径", GUILayout.Width(100f))) Process.Start(_saveCSharpPath);
//             // if (GUILayout.Button("生成", GUILayout.Width(100f))) ChangeCSharp(_saveCSharpPath);
//             // _saveCSharpPath = EditorGUILayout.TextField(_saveCSharpPath);
//             // EditorGUILayout.EndHorizontal();
//             //
//             // EditorGUILayout.BeginHorizontal();
//             // if (GUILayout.Button("选择存储路径", GUILayout.Width(70f))) _saveAssetPath = EditorUtility.OpenFolderPanel("选择文件夹", default, default);
//             // if (GUILayout.Button("打开Asset文件路径", GUILayout.Width(100f))) Process.Start(_saveAssetPath);
//             // if (GUILayout.Button("生成", GUILayout.Width(100f))) ChangeAsset(_saveAssetPath);
//             // _saveAssetPath = EditorGUILayout.TextField(_saveAssetPath);
//             // EditorGUILayout.EndHorizontal();
//
//             Repaint(); //实时刷新
//         }
//
//         private void ChangeJson(string path, string loadExcelPath)
//         {
//             // if (string.IsNullOrEmpty(loadExcelPath))
//             //     "请选择一个Excel文件".Log();
//             //
//             // path = $"{path}/{Path.GetFileNameWithoutExtension(loadExcelPath)}.json";
//             // if (File.Exists(path)) File.Delete(path);
//             // Dictionary<string, Dictionary<string, string>> temp = new Dictionary<string, Dictionary<string, string>>();
//             //
//             // string[] data1 = Data[0];
//             //
//             // for (int i = 3; i < Data.Length; i++)
//             // {
//             //     var data = Data[i];
//             //     temp.Add(data[1], new Dictionary<string, string>());
//             //     for (int j = 2; j < data.Length; j++)
//             //         temp[data[1]].Add(data1[j], data[j]);
//             // }
//             //
//             // string jsonStr = JsonMapper.ToJson(temp);
//             // File.WriteAllText(path, jsonStr);
//             // AssetDatabase.Refresh();
//             // "创建成功".Log();
//         }
//     }
// }
//
// namespace EditorTools
// {
//     /// <summary>
//     /// 数据选项
//     /// </summary>
//     public enum DataOperation
//     {
//         Binary,
//         Json,
//         PlayerPref,
//         XML,
//         Excel,
//     }
//
//     /// <summary>
//     /// 数据处理接口
//     /// </summary>
//     public interface IDataHandle
//     {
//         public void Save(object obj, string fileName);
//
//         public T Load<T>(string fileName) where T : class;
//
//         public void Debug(Object o);
//     }
//
//     /// <summary>
//     /// 数据拓展
//     /// </summary>
//     public static class DataExpand
//     {
//         public static void TestByte()
//         {
//             StringBuilder sb = new StringBuilder();
//             sb.AppendLine("有符号");
//             sb.AppendLine("sbyte" + sizeof(sbyte) + "字节");
//             sb.AppendLine("int" + sizeof(int) + "字节");
//             sb.AppendLine("short" + sizeof(short) + "字节");
//             sb.AppendLine("long" + sizeof(long) + "字节");
//             sb.AppendLine("无符号");
//             sb.AppendLine("byte" + sizeof(byte) + "字节");
//             sb.AppendLine("uint" + sizeof(uint) + "字节");
//             sb.AppendLine("ushort" + sizeof(ushort) + "字节");
//             sb.AppendLine("ulong" + sizeof(ulong) + "字节");
//             sb.AppendLine("浮点");
//             sb.AppendLine("float" + sizeof(float) + "字节");
//             sb.AppendLine("double" + sizeof(double) + "字节");
//             sb.AppendLine("decimal" + sizeof(decimal) + "字节");
//             sb.AppendLine("特殊");
//             sb.AppendLine("bool" + sizeof(bool) + "字节");
//             sb.AppendLine("char" + sizeof(char) + "字节");
//             Debug.Log(sb.ToString());
//         }
//     }
//
//     /// <summary>
//     /// 二进制数据处理
//     /// </summary>
//     public class BinaryHandle : IDataHandle
//     {
//         public void Save(object obj, string fileName)
//         {
//             throw new NotImplementedException();
//         }
//
//         public T Load<T>(string fileName) where T : class
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Debug(Object o)
//         {
//             throw new NotImplementedException();
//         }
//
//         /// <summary>
//         /// 不同变量类型 :有符号 sbyte int short long 无符号 byte uint ushort ulong 浮点 float double decimal 特殊 bool char string
//         /// 在内存中都以字节的形式存储着 1byte = 8bit 1bit(位)不是0就是1 通过sizeof方法可以看到常用变量类型占用的字节空间长度
//         /// 1.将各类型转字节byte[] bytes = BitConverter.GetBytes(256);
//         /// 2.字节数组转各类型 int i = BitConverter.ToInt32(bytes, 0);
//         /// 1.将字符串以指定编码格式转字节 byte[] bytes2 = Encoding.UTF8.GetBytes("测试");
//         /// 2.字节数组以指定编码格式转字符串 string s = Encoding.UTF8.GetString(bytes2);
//         /// </summary>
//         public void Creat()
//         {
//         }
//     }
//
//     /// <summary>
//     /// Json处理
//     /// </summary>
//     public class JsonHandle : IDataHandle
//     {
//         public void Save(object obj, string fileName)
//         {
//             throw new NotImplementedException();
//         }
//
//         public T Load<T>(string fileName) where T : class
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Debug(Object o)
//         {
//             throw new NotImplementedException();
//         }
//     }
//
//     public class ExcelHandle : IDataHandle
//     {
//         public void Save(object obj, string fileName)
//         {
//             throw new NotImplementedException();
//         }
//
//         public T Load<T>(string fileName) where T : class
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Debug(Object o)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }