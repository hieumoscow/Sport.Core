using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platform;
using Newtonsoft.Json.Linq;
using Sport.Core.Exceptions;
using Sport.Core.Mvvm.Validation;
using Sport.Core.Services.Interfaces;
using Sport.Core.Strings;

namespace Sport.Core.Mvvm
{
    public class LocalizationIndexed : MvxNotifyPropertyChanged
    {
        public IMvxLanguageBinder TextSource => new MvxLanguageBinder("", GetType().Name);
        public string this[string index] => AppResources.ResourceManager.GetString(index);
    }
    public class MyMvxViewModel : MvxViewModel
    {
        private string _title;

        public IMvxViewModel Parent { get; set; }

        public IMvxLanguageBinder TextSource => new MvxLanguageBinder("", GetType().Name);

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string this[string index] => AppResources.ResourceManager.GetString(index);


        public virtual void Init()
        {
        }


        protected async Task<T> Request<T>(Task<T> task, Action<T> completed = null)
        {
            try
            {
                Mvx.Resolve<IMessageService>().StartProgress(AppResources.Msg_Loading);
                var r = await task;
                completed?.Invoke(r);
                return r;
            }
            catch (ApiException<string> ex)
            {
                UpdateError(ex);
            }
            catch (ApiException<T> ex)
            {
                UpdateError(ex);
            }
            catch (Exception)
            {
                Debugger.Break();
            }
            finally
            {
                Mvx.Resolve<IMessageService>().StopProgress();
            }
            return default(T);
        }

        private void UpdateError<T>(ApiException<T> ex)
        {
            var array = ex.Response.ErrorDesc as JArray;
            if (array != null)
                foreach (var obj in array)
                {
                    UpdateError(obj);
                }
            else
            {
                UpdateError(ex.Response.ErrorDesc);
            }
        }

        protected async void UpdateError(JToken obj)
        {
            try
            {
                if (obj.Type == JTokenType.String)
                {
                    await Mvx.Resolve<IMessageService>().ShowMessageAsync(obj.ToString(), AppResources.Msg_Error);
                    return;
                }
                var v = obj.ToObject<KeyValuePair<string, string>>();
                foreach (var prop in GetType().GetProperties(BindingFlags.Public))
                {
                    if (prop.Name.Equals(v.Key) && prop.PropertyType == typeof(TextValue))
                    {
                        var textValue = prop.GetValue(this) as TextValue;
                        if (textValue != null)
                            textValue.ErrorMessage = v.Value;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

    }
}
