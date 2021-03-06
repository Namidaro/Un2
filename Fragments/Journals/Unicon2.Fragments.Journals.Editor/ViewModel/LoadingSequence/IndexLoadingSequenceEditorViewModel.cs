﻿using Unicon2.Fragments.Journals.Editor.Interfaces.LoadingSequence;
using Unicon2.Fragments.Journals.Infrastructure.Keys;
using Unicon2.Fragments.Journals.Infrastructure.Model.LoadingSequence;
using Unicon2.Infrastructure;
using Unicon2.Unity.ViewModels;

namespace Unicon2.Fragments.Journals.Editor.ViewModel.LoadingSequence
{
    public class IndexLoadingSequenceEditorViewModel : ViewModelBase, IIndexLoadingSequenceEditorViewModel
    {
        private ushort _journalStartAddress;
        private ushort _numberOfPointsInRecord;
        private ushort _wordFormatFrom;
        private ushort _wordFormatTo;
        private bool _isWordFormatNotForTheWholeRecord;
        private IIndexLoadingSequence _indexLoadingSequence;
        private ushort _indexWritingAddress;


        public string StrongName => JournalKeys.INDEX_LOADING_SEQUENCE +
                                    ApplicationGlobalNames.CommonInjectionStrings.EDITOR_VIEWMODEL;


        public object Model
        {
            get
            {
                this._indexLoadingSequence.IsWordFormatNotForTheWholeRecord = this.IsWordFormatNotForTheWholeRecord;
                this._indexLoadingSequence.JournalStartAddress = this.JournalStartAddress;
                this._indexLoadingSequence.NumberOfPointsInRecord = this.NumberOfPointsInRecord;
                this._indexLoadingSequence.WordFormatTo = this.WordFormatTo;
                this._indexLoadingSequence.WordFormatFrom = this.WordFormatFrom;
                this._indexLoadingSequence.IndexWritingAddress = this.IndexWritingAddress;
                this._indexLoadingSequence.WriteIndexOnlyFirstTime = this.WriteIndexOnlyFirstTime;

                return this._indexLoadingSequence;
            }
            set
            {
                this._indexLoadingSequence = value as IIndexLoadingSequence;
                this.IsWordFormatNotForTheWholeRecord = this._indexLoadingSequence.IsWordFormatNotForTheWholeRecord;
                this.JournalStartAddress = this._indexLoadingSequence.JournalStartAddress;
                this.NumberOfPointsInRecord = this._indexLoadingSequence.NumberOfPointsInRecord;
                this.WordFormatTo = this._indexLoadingSequence.WordFormatTo;
                this.WordFormatFrom = this._indexLoadingSequence.WordFormatFrom;
                this.IndexWritingAddress = this._indexLoadingSequence.IndexWritingAddress;
                WriteIndexOnlyFirstTime = this._indexLoadingSequence.WriteIndexOnlyFirstTime;

            }
        }
        private bool _writeIndexOnlyFirstTime;

        public bool WriteIndexOnlyFirstTime
        {
            get => _writeIndexOnlyFirstTime;
            set
            {
                _writeIndexOnlyFirstTime = value;
                RaisePropertyChanged();
            }
        }
        public ushort JournalStartAddress
        {
            get { return this._journalStartAddress; }
            set
            {
                this._journalStartAddress = value;
                this.RaisePropertyChanged();
            }
        }

        public ushort NumberOfPointsInRecord
        {
            get { return this._numberOfPointsInRecord; }
            set
            {
                this._numberOfPointsInRecord = value;
                this.RaisePropertyChanged();
            }
        }

        public ushort WordFormatFrom
        {
            get { return this._wordFormatFrom; }
            set
            {
                this._wordFormatFrom = value;
                this.RaisePropertyChanged();
            }
        }

        public ushort WordFormatTo
        {
            get { return this._wordFormatTo; }
            set
            {
                this._wordFormatTo = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsWordFormatNotForTheWholeRecord
        {
            get { return this._isWordFormatNotForTheWholeRecord; }
            set
            {
                this._isWordFormatNotForTheWholeRecord = value;
                this.RaisePropertyChanged();
            }
        }

        public ushort IndexWritingAddress
        {
            get { return this._indexWritingAddress; }
            set
            {
                this._indexWritingAddress = value;
                this.RaisePropertyChanged();
            }
        }


        public string NameForUiKey => this._indexLoadingSequence.StrongName;
    }
}
