using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using Prism.Mvvm;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sample.Models;
using Sample.ViewModels.ValidationAttributes;

namespace Sample.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        //TODO: 言語リソースからメッセージを取得する場合はこれ
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Lang))] 
        [Required(ErrorMessage = "必須")]
        [StringLength(20, ErrorMessage = "20文字以内で入力してください")]
        public ReactiveProperty<string> Title { get; }
        public ReadOnlyReactiveProperty<string> TitleError { get; }
        [StringLength(140, ErrorMessage = "140文字以内で入力してください")]
        public ReactiveProperty<string> Description { get; }
        public ReadOnlyReactiveProperty<string> DescriptionError { get; }
        //[Range(0,999,ErrorMessage = "0-999の範囲で入力してください")]
        [RegularExpression(@"[0-9]{1,3}", ErrorMessage = "3桁以内の整数で入力してください")]
        public ReactiveProperty<string> Priority { get; }
        public ReadOnlyReactiveProperty<string> PriorityError { get; }

        public ReactiveProperty<DateTime> DateTo { get; }
        public ReactiveProperty<DateTime> DateFrom { get; }

        [CombineCompareAsDate(Mode.LessThanOrEqual, ErrorMessage = "終了日は開始日以降にしてください")]
        public ReactiveProperty<IList<DateTime>> CombineDate { get; }
        public ReadOnlyReactiveProperty<string> DateError { get; }

        public AsyncReactiveCommand OKCommand { get; set; }

        public MainPageViewModel(IPageDialogService pageDialog, ToDo todo)
        {

            Title = todo.ToReactivePropertyAsSynchronized(x => x.Title, ignoreValidationErrorValue: true) //ModelのTitleとシンクロして、エラー以降の値はModelにセットしない
                        .SetValidateAttribute(() => this.Title);    //Validationセット

            Description = todo.ToReactivePropertyAsSynchronized(x => x.Descriptions, ignoreValidationErrorValue: true)
                              .SetValidateAttribute(() => this.Description);

            Priority = todo.ToReactivePropertyAsSynchronized(
                                x => x.Priority,
                                x => x.ToString(),  // M->VMは文字列変換
                                x => {
                                    // VM->Mは数値変換
                                    int ret;
                                    int.TryParse(x, out ret);
                                    return ret;
                                },
                                ignoreValidationErrorValue: true
                            )
                           .SetValidateAttribute(() => this.Priority);

            TitleError = Title.ObserveErrorChanged
                              .Select(x => x?.Cast<string>()?.FirstOrDefault()) //発生したエラーの最初の値を文字列として取得
                              .ToReadOnlyReactiveProperty();


            DescriptionError = Description.ObserveErrorChanged
                                          .Select(x => x?.Cast<string>()?.FirstOrDefault())
                                          .ToReadOnlyReactiveProperty();

            PriorityError = Priority.ObserveErrorChanged
                                          .Select(x => x?.Cast<string>()?.FirstOrDefault())
                                          .ToReadOnlyReactiveProperty();

            DateTo = todo.ToReactivePropertyAsSynchronized(x => x.DateTo, ignoreValidationErrorValue: true);
            DateFrom = todo.ToReactivePropertyAsSynchronized(x => x.DateFrom, ignoreValidationErrorValue: true);

            CombineDate = new[]{
                DateFrom,
                DateTo
            }.CombineLatest()   //開始日、終了日をドッキングして
             .ToReactiveProperty() //ReactiveProperty化して
             .SetValidateAttribute(() => this.CombineDate); //カスタムルールを適用

            //それをエラーに流す
            DateError = CombineDate.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();

            OKCommand = new[]{
                Title.ObserveHasErrors,
                Description.ObserveHasErrors,
                Priority.ObserveHasErrors,
                CombineDate.ObserveHasErrors
            }.CombineLatest(x => x.All(y => !y)).ToAsyncReactiveCommand();  //エラーをまとめてCommand化


            OKCommand.Subscribe(async _ => {
                await pageDialog.DisplayAlertAsync("", "Done", "OK");
            });
        }


    }
}

