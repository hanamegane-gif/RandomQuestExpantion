using System;
using System.Collections.Generic;

namespace RandomQuestExpantion.General
{
    internal class DramaWrapper
    {
        const string __MOD_EXCEL_NAME__ = "RQXDialog";

        internal static HashSet<string> ArgumentFlagsDefinition => new HashSet<string>
        {
            "RQX_canraise", // 交渉吊り上げ可能
        };

        internal static HashSet<string> ArgumentIntegerDefinition => new HashSet<string>
        {
            // ないよ！
        };

        internal static HashSet<string> ResultsDefinition => new HashSet<string>
        {
            "RQX_negoaccept", // 交渉吊り上げ可能
            "RQX_negoaccept", // 交渉成功
            "RQX_negoraise", // 交渉吊り上げ成功
        };

        // Lock
        // v
        // SetArgument, SetCallbackAction
        // v
        // PlayDrama
        // v
        // Callback, GetDramaResult
        // v
        // Release
        // v
        // Lock
        private enum State
        {
            UnInit,
            Free,
            Ready,
            DramaPlaying,
            DramaEnd,
        }

        internal static Action CallbackAction => LayerDrama.refAction1;

        internal static Action CallbackSubAction => LayerDrama.refAction2;

        internal static string StateOfDrama => nameof(_StateOfDrama);

        private static State _StateOfDrama = State.UnInit;

        internal static void Lock()
        {
            if (_StateOfDrama > State.Free)
            {
                throw new InvalidOperationException();
            }

            CleanArgumentsAndResults();
            _StateOfDrama = State.Ready;
        }

        internal static void Release()
        {
            if (_StateOfDrama <= State.Free)
            {
                throw new InvalidOperationException();
            }

            CleanArgumentsAndResults();
            _StateOfDrama = State.Free;
        }

        internal static void SetArgumentBoolean(string name, bool value)
        {
            if (_StateOfDrama != State.Ready)
            {
                throw new InvalidOperationException();
            }

            if (!ArgumentFlagsDefinition.Contains(name))
            {
                throw new ArgumentException(name);
            }

            if (EMono.player.dialogFlags.ContainsKey(name))
            {
                EMono.player.dialogFlags[name] = (value) ? 1 : 0;
            }
            else
            {
                EMono.player.dialogFlags.Add(name, (value) ? 1 : 0);
            }
        }

        internal static void SetArgumentInteger(string name, int value)
        {
            if (_StateOfDrama != State.Ready)
            {
                throw new InvalidOperationException();
            }

            if (!ArgumentIntegerDefinition.Contains(name))
            {
                throw new ArgumentException(name);
            }

            if (EMono.player.dialogFlags.ContainsKey(name))
            {
                EMono.player.dialogFlags[name] = value;
            }
            else
            {
                EMono.player.dialogFlags.Add(name, value);
            }
        }

        // 文字列引数は最大5個(refDrama1~5)までで固定
        internal static void SetArgumentStrings(string arg1, string arg2 = null, string arg3 = null, string arg4 = null, string arg5 = null)
        {
            if (_StateOfDrama != State.Ready)
            {
                throw new InvalidOperationException();
            }

            GameLang.refDrama1 = arg1;
            GameLang.refDrama2 = arg2;
            GameLang.refDrama3 = arg3;
            GameLang.refDrama4 = arg4;
            GameLang.refDrama5 = arg5;
        }

        internal static void SetCallbackAction(Action callback)
        {
            if (_StateOfDrama != State.Ready)
            {
                throw new InvalidOperationException();
            }

            LayerDrama.refAction1 = callback;
        }

        internal static void SetCallbackSubAction(Action callback)
        {
            if (_StateOfDrama != State.Ready)
            {
                throw new InvalidOperationException();
            }

            LayerDrama.refAction2 = callback;
        }

        // ナレーションや3人以上のドラマを再生するつもりは今のところないので、talkerは常に1人とする
        internal static void PlayDrama(Chara talker, string startStep, string excelName = __MOD_EXCEL_NAME__)
        {
            if (_StateOfDrama != State.Ready)
            {
                throw new InvalidOperationException();
            }

            _StateOfDrama = State.DramaPlaying;
            talker.ShowDialog(excelName, startStep);
            _StateOfDrama = State.DramaEnd;
        }

        // DramaのSetFlagはintのみなのでintかboolしか返さない
        internal static T GetDramaResult<T>(string name)
        {
            if ((typeof(T) != typeof(int) && typeof(T) != typeof(bool)))
            {
                throw new InvalidOperationException("RandomQuestExpansion: ンアー！製作者の頭が悪すぎます！");
            }

            if (_StateOfDrama < State.DramaEnd)
            {
                throw new InvalidOperationException();
            }

            if (EMono.player.dialogFlags.TryGetValue(name, out int val))
            {
                return (typeof(T) == typeof(bool)) ? (T)(object)(val != 0) : (T)(object)val;
            }
            else
            {
                return default(T);
            }
        }

        private static void CleanArgumentsAndResults()
        {
            CleanFlagArguments();
            CleanIntegerArguments();
            CleanStringArguments();
            ClearCallback();
            CleanResults();
        }

        private static void CleanFlagArguments()
        {
            foreach (var key in ArgumentFlagsDefinition)
            {
                if (EMono.player.dialogFlags.ContainsKey(key))
                {
                    EMono.player.dialogFlags[key] = 0;
                }
                else
                {
                    EMono.player.dialogFlags.Add(key, 0);
                }
            }
        }

        private static void CleanIntegerArguments()
        {
            foreach (var key in ArgumentIntegerDefinition)
            {
                if (EMono.player.dialogFlags.ContainsKey(key))
                {
                    EMono.player.dialogFlags[key] = 0;
                }
                else
                {
                    EMono.player.dialogFlags.Add(key, 0);
                }
            }
        }

        private static void CleanStringArguments()
        {
            GameLang.ClearDramaRef();
        }

        private static void ClearCallback()
        {
            LayerDrama.refAction1 = () => { return; };
            LayerDrama.refAction2 = () => { return; };
        }

        private static void CleanResults()
        {
            foreach (var key in ResultsDefinition)
            {
                if (EMono.player.dialogFlags.ContainsKey(key))
                {
                    EMono.player.dialogFlags[key] = 0;
                }
                else
                {
                    EMono.player.dialogFlags.Add(key, 0);
                }
            }
        }
    }
}
