//------------------------------------------------------------
// Any Localization
// Author: George Huan
// Date: 2020-11-12
// Homepage: 
// Feedback: 
//------------------------------------------------------------


using UnityEngine;
using UnityEditor;

using System.IO;
using System.Xml;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using OfficeOpenXml;


namespace AnyLocalization
{
    public class AnyLocalizationEditorWindow : EditorWindow
    {
        [MenuItem("Window/Any Localization")]
        private static void Init()
        {
            EditorWindow localizationEditorWindow = GetWindow(typeof(AnyLocalizationEditorWindow), false, "Any Localization", true);
            localizationEditorWindow.minSize = new Vector2(800, 950);
        }

        void OnEnable()
        {
            LoadXmlFiles();
            KVSort();
        }

        private void OnDisable()
        {
            EditorPrefs.SetString("XMLDictionariesPath", XMLDictionariesPath);
            SaveXmlFiles();
        }

        void OnDestroy()
        {
            strKeyValuePairs.Clear();
            stringList.Clear();
            overrideKey = string.Empty;
        }

        private static GUIStyle textFieldSytle;
        private static GUIStyle lableFieldSytle;

        public static string XMLDictionariesPath;

        public static List<Language> languages = new List<Language>();
        public static Dictionary<string, Dictionary<Language, string>> strKeyValuePairs = new Dictionary<string, Dictionary<Language, string>>();

        private static string[] languagesOptions;
        private static int languagesOptionsIndex = 0;

        private static string searchText = string.Empty;
        private static bool drawUndefined = false;

        private static string xmlPath;
        private static XmlDocument xmlDocument = new XmlDocument();

        void OnGUI()
        {
            textFieldSytle = GUI.skin.textField;
            textFieldSytle.wordWrap = true;
            textFieldSytle.richText = false;

            lableFieldSytle = GUI.skin.label;
            lableFieldSytle.richText = true;

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                XMLDictionariesPath = EditorGUILayout.TextField("XML Dictionaries Path: ", XMLDictionariesPath);
                if (GUILayout.Button("Select", GUILayout.MinHeight(15), GUILayout.MaxWidth(80)))
                {
                    XMLDictionariesPath = EditorUtility.OpenFolderPanel("Select XML Dictionaries Folder", Application.dataPath, "Dictionaries");
                    EditorPrefs.SetString("XMLDictionariesPath", XMLDictionariesPath);
                    LoadXmlFiles();
                }
                if (XMLDictionariesPath == "")
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.HelpBox("Select XML Dictionaries Folder First", MessageType.Info);
                    
                    return;
                }
                if (languages.Count == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.HelpBox("Can't find any language dictionary under the path!", MessageType.Warning);
                    return;
                }
                if (XMLDictionariesPath != "")
                {
                    if (GUILayout.Button("Languages", GUILayout.MinHeight(15), GUILayout.MaxWidth(120)))
                    LanguageEditorWindow.Init();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            DrawToolbar();
            DrawTextScrollView();
            DrawTextEditingZone();
            DrawActionBar();

            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
                EditorGUI.FocusTextInControl(null);
                GUIUtility.keyboardControl = 0;
            }
        }


        /// <summary>
        /// 绘制工具栏
        /// </summary>
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Search Dictionary Key or Value:");
                languagesOptionsIndex = EditorPrefs.GetInt("DisplayLanguage");
                if (languagesOptionsIndex >= languages.Count)
                {
                    languagesOptionsIndex = 0;
                }
                languagesOptionsIndex = EditorGUILayout.Popup("             Display Language：", languagesOptionsIndex, languagesOptions, GUILayout.MaxWidth(400));
                EditorPrefs.SetInt("DisplayLanguage", languagesOptionsIndex);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            searchText = DrawSearchField(searchText);
            drawUndefined = EditorGUILayout.ToggleLeft("Show Undefined", drawUndefined, GUILayout.MaxWidth(115));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制搜索框
        /// </summary>
        /// <param name="text"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private string DrawSearchField(string text, params GUILayoutOption[] options)
        {
            MethodInfo info = typeof(EditorGUILayout).GetMethod("ToolbarSearchField", BindingFlags.NonPublic | BindingFlags.Static, null, new System.Type[] { typeof(string), typeof(GUILayoutOption[]) }, null);
            if (info != null)
            {
                text = (string)info.Invoke(null, new object[] { text, options });
            }
            return text;
        }

        const int rectY = 90;
        const int maxRow = 15;
        const float btnHeight = 22f;
        static Vector2 textScrollViewPos;

        /// <summary>
        /// 绘制Text滚动视图
        /// </summary>
        private void DrawTextScrollView()
        {
            int kvCount = strKeyValuePairs.Count();

            List<int> kvIndexList = new List<int>();

            for (int i = 0; i < kvCount; i++)
            {
                if (KVQuery(strKeyValuePairs.ElementAt(i)))
                {
                    kvIndexList.Add(i);
                }
            }

            Rect scrollViewRect = new Rect(0, rectY, position.width, (maxRow + 1) * btnHeight);
            Rect scrollContentRect = new Rect(0, rectY, position.width - 20, kvIndexList.Count * btnHeight);

            textScrollViewPos = GUI.BeginScrollView(scrollViewRect, textScrollViewPos, scrollContentRect);

            int beginBtnIndex = (int)(textScrollViewPos.y / btnHeight);
            int endBtnIndext = beginBtnIndex + maxRow;

            int drawCount = endBtnIndext - beginBtnIndex + 1;
            if (drawCount > kvIndexList.Count) drawCount = kvIndexList.Count;

            for (int j = 0; j < drawCount; j++)
            {
                var item = strKeyValuePairs.ElementAt(kvIndexList[beginBtnIndex + j]);
                EditorGUILayout.BeginHorizontal();
                {
                    item.Value.TryGetValue(languages[languagesOptionsIndex], out string btnValue);

                    Rect rect = new Rect(0, rectY + textScrollViewPos.y + j * btnHeight, scrollViewRect.width, btnHeight);

                    if (GUI.Button(rect, GUIContent.none))
                    {
                        stringList = new List<string>();

                        originKey = item.Key;
                        overrideKey = item.Key;

                        for (int i = 0; i < languages.Count; i++)
                        {
                            item.Value.TryGetValue(languages[i], out string subValue);
                            stringList.Add(subValue);
                        }
                        GUIUtility.keyboardControl = 0;
                    }
                    rect.x += 5;
                    EditorGUI.LabelField(rect, item.Key);
                    rect.x += 300;
                    EditorGUI.LabelField(rect, btnValue, lableFieldSytle);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.EndScrollView();
            EditorGUILayout.Space();

            DrawAddButton(new Rect(0, 4 + rectY + drawCount * btnHeight, position.width, 22.5f));
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// 绘制Add按钮
        /// </summary>
        /// <returns></returns>
        private void DrawAddButton(Rect rect)
        {
            if (GUI.Button(rect, "Add Key"))
            {
                GUIUtility.keyboardControl = 0;
                Dictionary<Language, string> kv = new Dictionary<Language, string>();

                for (int i = 0; i < languages.Count; i++)
                {
                    kv.Add(languages[i], string.Empty);
                }

                string key = ".Unnamed." + System.DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss fff");
                overrideKey = originKey = key;

                strKeyValuePairs.Add(key, kv);
                KVSort();
                textScrollViewPos.y = 0;
                stringList = new List<string>();
                for (int i = 0; i < languages.Count; i++) stringList.Add(string.Empty);
            }
        }

        static string originKey;
        static string overrideKey;
        static List<string> stringList = new List<string>();

        /// <summary>
        /// 绘制Text编辑区
        /// </summary>
        private void DrawTextEditingZone()
        {
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("Key");
                EditorGUILayout.BeginHorizontal();
                {
                    overrideKey = GUILayout.TextField(overrideKey, GUILayout.MinWidth(250));
                    if (GUILayout.Button("Copy"))
                    {
                        TextEditor te = new TextEditor();
                        te.text = overrideKey;
                        te.SelectAll();
                        te.Copy();
                        Debug.Log("Copy: " + overrideKey);
                    }
                    if (GUILayout.Button("Rename"))
                    {
                        strKeyValuePairs = strKeyValuePairs.ToDictionary(k => k.Key == originKey ? overrideKey : k.Key, k => k.Value);
                        originKey = overrideKey;
                    }
                    if (GUILayout.Button("Revert"))
                    {
                        overrideKey = originKey;
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        strKeyValuePairs.Remove(originKey);
                        stringList.Clear();
                        overrideKey = string.Empty;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (stringList.Count == 0)
                {
                    EditorGUILayout.EndVertical();
                    return;
                }

                for (int i = 0; i < languages.Count; i++)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("<b>" + languages[i].ToString() + "</b>", lableFieldSytle);
                    strKeyValuePairs[originKey][languages[i]] = stringList[i] = EditorGUILayout.TextField(stringList[i], textFieldSytle, GUILayout.MinHeight(50));
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制操作栏
        /// </summary>
        private void DrawActionBar()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save to StreamingAssets", GUILayout.MinHeight(22.5f), GUILayout.MinWidth(300)))
                {
                    if (overrideKey != string.Empty)
                    {
                        strKeyValuePairs = strKeyValuePairs.ToDictionary(k => k.Key == originKey ? overrideKey : k.Key, k => k.Value);
                        originKey = overrideKey;
                    }
                    SaveXmlFiles();
                    SaveXmlFiles2StreamingAssets();
                }
                if (GUILayout.Button("Import xlsx", GUILayout.MinHeight(22.5f))) ImportXlsx();
                if (GUILayout.Button("Export xlsx", GUILayout.MinHeight(22.5f))) ExportXlsx();
                if (GUILayout.Button("Filter Char", GUILayout.MinHeight(22.5f))) FilterChar();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 加载XML
        /// </summary>
        public static void LoadXmlFiles()
        {
            languages = new List<Language>();
            strKeyValuePairs = new Dictionary<string, Dictionary<Language, string>>();

            XMLDictionariesPath = EditorPrefs.GetString("XMLDictionariesPath");
            if (XMLDictionariesPath == "")
            {
                return;
            }

            foreach (var item in Directory.GetFiles(XMLDictionariesPath))
            {
                if (Path.GetExtension(item).ToLower() == ".xml")
                {
                    languages.Add((Language)System.Enum.Parse(typeof(Language), Path.GetFileNameWithoutExtension(item)));
                }
            }


            languagesOptions = new string[languages.Count];

            for (int i = 0; i < languages.Count; i++)
            {
                languagesOptions[i] = languages[i].ToString();
                xmlPath = $"{XMLDictionariesPath}/{languages[i]}.xml";
                if (File.Exists(xmlPath))
                {
                    xmlDocument.Load(xmlPath);
                    XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("Dictionaries/Dictionary").ChildNodes;
                    foreach (XmlNode item in xmlNodeList)
                    {

                        string key = item.Attributes["Key"].Value;
                        string value = item.Attributes["Value"].Value;

                        Dictionary<Language, string> kv = new Dictionary<Language, string>();

                        if (strKeyValuePairs.TryGetValue(key, out kv))
                        {
                            kv.Add(languages[i], value);
                        }
                        else
                        {
                            kv = new Dictionary<Language, string>();
                            kv.Add(languages[i], value);
                            if (item.Name == "String") strKeyValuePairs.Add(key, kv);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 保存XML
        /// </summary>
        private static void SaveXmlFiles()
        {
            xmlDocument = new XmlDocument();
            for (int i = 0; i < languages.Count; i++)
            {
                xmlPath = $"{XMLDictionariesPath}/{languages[i]}.xml";
                if (File.Exists(xmlPath))
                {
                    xmlDocument.Load(xmlPath);
                    xmlDocument.SelectSingleNode("Dictionaries/Dictionary").RemoveAll();
                    XmlElement xmlElement = (XmlElement)xmlDocument.SelectSingleNode("Dictionaries/Dictionary");
                    xmlElement.SetAttribute("Language", languages[i].ToString());

                    foreach (var item in strKeyValuePairs)
                    {
                        item.Value.TryGetValue(languages[i], out string v);
                        XmlElement element = xmlDocument.CreateElement("String");
                        element.SetAttribute("Key", item.Key);
                        element.SetAttribute("Value", v);
                        xmlElement.AppendChild(element);
                    }
                    xmlDocument.Save(xmlPath);
                }
            }
        }

        /// <summary>
        /// 保存XML到StreamingAssets
        /// </summary>
        public static void SaveXmlFiles2StreamingAssets()
        {
            string streamingAssetsFolder = $"{Application.streamingAssetsPath}/{ANL.XMLStreamingAssetsPath}";

            if (!File.Exists(streamingAssetsFolder)) Directory.CreateDirectory(streamingAssetsFolder);
            foreach (var item in Directory.GetFiles(XMLDictionariesPath))
            {
                if (Path.GetExtension(item).ToLower() == ".xml")
                {
                    File.Copy(item, $"{streamingAssetsFolder}/{Path.GetFileName(item)}", true);
                }
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void OpenDirectory(string path)
        {
            Thread newThread = new Thread(new ParameterizedThreadStart(CmdOpenDirectory));
            newThread.Start(path);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="obj"></param>
        private static void CmdOpenDirectory(object obj)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c start " + obj.ToString();
            Debug.Log(p.StartInfo.Arguments);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.WaitForExit();
            p.Close();
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        private void ExportXlsx()
        {
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(System.DateTime.Now.ToString("yyyy_MM_dd"));

                worksheet.Cells[1, 1].Value = "Key";
                for (int i = 0; i < languages.Count; i++)
                {
                    worksheet.Cells[1, i + 2].Value = languages[i].ToString();
                }

                xmlDocument = new XmlDocument();
                for (int i = 0; i < languages.Count; i++)
                {
                    int row = 2;
                    foreach (var item in strKeyValuePairs)
                    {
                        item.Value.TryGetValue(languages[i], out string v);
                        worksheet.Cells[row, 1].Value = item.Key;
                        worksheet.Cells[row, i + 2].Value = v;
                        row++;
                    }
                }

                var xlFile = new FileInfo("Any Localization - " + System.DateTime.Now.ToString("yyyy MM dd") + ".xlsx");
                package.SaveAs(xlFile);
            }
            OpenDirectory(Application.dataPath);
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        private void ImportXlsx()
        {
            string xlFile = EditorUtility.OpenFilePanel("请选择要导入的Excel文件！", Application.dataPath, "xlsx");

            if (xlFile.Length == 0)
            {
                return;
            }

            using (var package = new ExcelPackage(new FileInfo(xlFile)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int row = 2;

                while (worksheet.Cells[row, 1].Value != null)
                {
                    string key = worksheet.Cells[row, 1].Value.ToString();
                    //Debug.Log(key);

                    if (strKeyValuePairs.TryGetValue(key, out Dictionary<Language, string> kv))
                    {
                        for (int i = 0; i < languages.Count; i++)
                        {
                            string value = string.Empty;
                            if (worksheet.Cells[row, i + 2].Value != null) value = worksheet.Cells[row, i + 2].Value.ToString();
                            //Debug.Log(value);

                            if (kv.TryGetValue(languages[i], out string v))
                            {
                                strKeyValuePairs[key][languages[i]] = value;
                            }
                            else
                            {
                                kv.Add(languages[i], value);
                            }
                        }
                    }
                    else
                    {
                        Dictionary<Language, string> new_kv = new Dictionary<Language, string>();
                        for (int i = 0; i < languages.Count; i++)
                        {
                            string value = worksheet.Cells[row, i + 2].Value.ToString();
                            //Debug.Log(value);
                            new_kv.Add(languages[i], value);
                        }
                        strKeyValuePairs.Add(key, new_kv);
                    }
                    row++;
                }
            }
            SaveXmlFiles();
            LoadXmlFiles();
        }

        const string Roman_Number = "ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫLCDM";
        const string Math_Symbols = "≈≡≠＝≤≥＜＞≮≯∷±＋－×÷／∫∮∝∞∧∨∑∏∪∩∈∵∴⊥∥∠⌒⊙≌∽√";
        const string Punctuation = "。，、；：？！…—·ˉ¨‘’“”々～‖∶＂＇｀｜〃〔〕〈〉《》「」『』．〖〗【】（）［］｛｝,./?<>;:'\"|[]{}·﹒`~!@#$%^&*()_1234567890℃－γ¿¡„£";
        const string Chinese_Phonetic_Alphabet = "āáǎàōóǒòêēéěèīíǐìūúǔùǖǘǚǜü";
        const string EN = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        const string JA = "ぁぃぅぇぉかきくけこんさしすせそたちつってとゐなにぬねのはひふへほゑまみむめもゃゅょゎをあいうえおがぎぐげござじずぜぞだぢづでどぱぴぷぺぽばびぶべぼらりるれろやゆよわァィゥヴェォカヵキクケコサシスセソタチツッテトヰンナニヌネノハヒフヘホヱマミムメモャュョヮヲアイウエオガギグゲゴザジズゼゾダヂヅデドパピプペポバビブベボラリルレロヤユヨワ";
        const string KO = "ㅏㅑㅓㅕㅗㅛㅜㅠㅡㅣㄱ가갸거겨고교구규그기ㄴ나냐너녀노뇨누뉴느니ㄷ다댜더뎌도됴두듀드디ㄹ라랴러려로료루류르리ㅁ마먀머며모묘무뮤므미ㅂ바뱌버벼보뵤부뷰브비ㅅ사샤서셔소쇼수슈스시ㅇ아야어여오요우유으이ㅈ자쟈저져조죠주쥬즈지ㅊ차챠처쳐초쵸추츄츠치ㅋ카캬커켜코쿄쿠큐크키ㅌ타탸터텨토툐투튜트티ㅍ파퍄퍼펴포표푸퓨프피ㅎ하햐허혀호효후휴흐히ㄲ까꺄꺼껴꼬꾜꾸뀨끄끼ㄸ따땨떠뗘또뚀뚜뜌뜨띠ㅃ빠뺘뻐뼈뽀뾰뿌쀼쁘삐ㅆ싸쌰써쎠쏘쑈쑤쓔쓰씨ㅉ짜쨔쩌쪄쪼쬬쭈쮸쯔찌ㅐㅒㅔㅖㅘㅙㅚㅝㅞㅟㅢ개걔게계과괘괴궈궤귀긔내냬네녜놔놰뇌눠눼뉘늬대댸데뎨돠돼되둬뒈뒤듸래럐레례롸뢔뢰뤄뤠뤼릐매먜메몌뫄뫠뫼뭐뭬뮈믜배뱨베볘봐봬뵈붜붸뷔븨새섀세셰솨쇄쇠숴쉐쉬싀애얘에예와왜외워웨위의재쟤제졔좌좨죄줘줴쥐즤채챼체쳬촤쵀최춰췌취츼캐컈케켸콰쾌쾨쿼퀘퀴킈태턔테톄톼퇘퇴퉈퉤튀틔패퍠페폐퐈퐤푀풔풰퓌픠해햬헤혜화홰회훠훼휘희깨꺠께꼐꽈꽤꾀꿔꿰뀌끠때떄떼뗴똬뙈뙤뚸뛔뛰띄빼뺴뻬뼤뽜뽸뾔뿨쀄쀠쁴쌔썌쎄쎼쏴쐐쐬쒀쒜쒸씌째쨰쩨쪠쫘쫴쬐쭤쮀쮜쯰각간갇갈감갑강낙난낟날남납낭닥단닫달담답당락란랃랄람랍랑막만맏말맘맙망박반받발밤밥방삭산삳살삼삽상악안앋알암압앙작잔잗잘잠잡장착찬찯찰참찹창칵칸칻칼캄캅캉탁탄탇탈탐탑탕팍판팓팔팜팝팡학한핟할함합항";
        const string Language_Dorpdown = "中文简体繁體한국어";

        /// <summary>
        /// 生成去重字符集
        /// </summary>
        private void FilterChar()
        {
            string @default = Language_Dorpdown + Roman_Number + Math_Symbols + Punctuation + EN;

            foreach (var item in strKeyValuePairs)
            {
                @default += item.Key;
            }

            foreach (var item in strKeyValuePairs)
            {
                for (int i = 0; i < languages.Count; i++)
                {
                    if (item.Value.TryGetValue(languages[i], out string v))
                    {
                        @default += v;
                    }
                }
            }

            IEnumerable<char> distinctList = @default.Distinct();
            string characters = string.Join("", distinctList);

            Debug.Log(@default);
            Debug.Log(characters);

            string path = Application.dataPath + "/GameMain/Fonts/Custom Characters.txt";
            StreamWriter sw = File.CreateText(path);
            sw.Write(characters);
            sw.Close();

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 键值对查询
        /// </summary>
        /// <param name="kv"></param>
        /// <returns></returns>
        public static bool KVQuery(KeyValuePair<string, Dictionary<Language, string>> kv)
        {
            if (drawUndefined)
            {
                for (int i = 0; i < languages.Count; i++)
                {
                    kv.Value.TryGetValue(languages[i], out string v);
                    if (v == string.Empty) return true;
                }
                return false;
            }

            if (searchText == string.Empty) return true;

            if (kv.Key.ToLower().Contains(searchText.ToLower())) return true;

            for (int i = 0; i < languages.Count; i++)
            {
                kv.Value.TryGetValue(languages[i], out string v);
                if (v.Contains(searchText)) return true;
            }
            return false;
        }

        /// <summary>
        /// 键值对排序
        /// </summary>
        private void KVSort()
        {
            var keyValuePairs = from pair in strKeyValuePairs orderby pair.Key ascending select pair;
            strKeyValuePairs = keyValuePairs.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
