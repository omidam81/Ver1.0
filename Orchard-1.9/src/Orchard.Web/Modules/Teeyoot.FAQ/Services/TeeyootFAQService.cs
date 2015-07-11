using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.FAQ.Services
{
    public class TeeyootFAQService : ITeeyootFaqService
    {
        private readonly IRepository<FaqSectionRecord> _sectionRepository;
        private readonly IContentManager _contentManager;
        private readonly ILanguageService _languageService;

        public TeeyootFAQService(IRepository<FaqSectionRecord> sectionRepository, IContentManager contentManager, ILanguageService languageService)
        {
            _sectionRepository = sectionRepository;
            _contentManager = contentManager;
            _languageService = languageService;
        }

        public IEnumerable<FaqSectionRecord> GetFaqSections()
        {
            return _sectionRepository.Table.ToList();
        }

        public FaqSectionRecord GetDefaultSection()
        {
            //TODO: eugene: implement if needed
            return _sectionRepository.Table.FirstOrDefault();
        }

        public FaqSectionRecord GetFaqSectionById(int id)
        {
            return _sectionRepository.Get(id);
        }

        public FaqEntryPart CreateFaqEntry(string question, int sectionId, string languageCode, string answer = "")
        {
            var section = GetFaqSectionById(sectionId);
            var language = _languageService.GetLanguageByCode(languageCode);

            var faqEntryPart = _contentManager.Create<FaqEntryPart>("FaqEntry",
                fe =>
                {
                    fe.Question = question;
                    fe.Answer = answer;
                    fe.Section = section;
                    fe.Language = language;
                });

            return faqEntryPart;
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries()
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>();
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries(string language)
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>().Where(fe => fe.LanguageRecord.Code == language);
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries(int section)
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>().Where(fe => fe.FaqSectionRecord.Id == section);
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries(string language, int section)
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>().Where(fe => fe.LanguageRecord.Code == language && fe.FaqSectionRecord.Id == section);
        }

        public IEnumerable<FaqSectionRecord> GetFaqSectionsMoq()
        {
            var array = new FaqSectionRecord[] {
                
                new FaqSectionRecord 
                {
                    Id = 1,
                    Name = "Section 1",
                    Entries = new FaqEntryPartRecord[] {
                        new FaqEntryPartRecord
                        {
                            Id = 1,
                            Question = "Question 1"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 2"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 3"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 4"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 5"
                        }

                    }
                },
                new FaqSectionRecord 
                {
                    Id = 1,
                    Name = "Section 2",
                    Entries = new FaqEntryPartRecord[] {
                        new FaqEntryPartRecord
                        {
                            Id = 1,
                            Question = "Question 2.1"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 2.2"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 2.3"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 2.4"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 2.5"
                        }

                    }
                },
                new FaqSectionRecord 
                {
                    Id = 1,
                    Name = "Section 3",
                    Entries = new FaqEntryPartRecord[] {
                        new FaqEntryPartRecord
                        {
                            Id = 1,
                            Question = "Question 3.1"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 3.2"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 3.3"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 3.4"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 3.5"
                        }

                    }
                },
                new FaqSectionRecord 
                {
                    Id = 1,
                    Name = "Section 4",
                    Entries = new FaqEntryPartRecord[] {
                        new FaqEntryPartRecord
                        {
                            Id = 1,
                            Question = "Question 4.1"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 4.2"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 4.3"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 4.4"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 4.5"
                        }

                    }
                }
                
                
            };
            return array;
        }

        public FaqEntryPartRecord GetFaqEntryPartRecordById(int id)
        {
            var record = new FaqEntryPartRecord
                       {
                           Id = 1,
                           Question = "Question 1",
                           Answer = "<p>Ansver<p/><p> Some text jdjfhdfhdofh odhfgodhfg dhfghdfhdfh dhfod dhfig dfg dfg dfg dfgb dfg df gd fgd fgdf <br/> dfhskdfhksdhf df df df df df df df df<p/><h1>Some text<h1/><p>jdfgjdhfj dfg dfg dfg <p/>"+
                           
                           " <p> dfg df g dfg dfg df gd fgdfgdfg <p/> <h1> dfgdfgdfgdf <h1/> <p style='color: red'>dfs dfg dfg dfg df gdf gdf g<p/><span>dfgdfgdf fgghf f fg fg<span/>",
                           FaqSectionRecord = new FaqSectionRecord
                           {
                               Name = "Section 1",
                               Id = 1 
                           },
                           LanguageRecord = new LanguageRecord
                           {
                               Name = "eng"
                           }
                       };
            return record;

        }

        public FaqSectionRecord GetFaqSectionRecordById(int id)
        {
            var record = new FaqSectionRecord
            {
                Id = 1,
                Name = "Section 1",
                 Entries = new FaqEntryPartRecord[] {
                        new FaqEntryPartRecord
                        {
                            Id = 1111,
                            Question = "Question 1",
                            Answer = "<p>Ansver<p/><p> Some text jdjfhdfhdofh odhfgodhfg dhfghdfhdfh dhfod dhfig dfg dfg dfg dfgb dfg df gd fgd fgdf  dfhskdfhksdhf df df df df df df df df<p/><h1>Some text<h1/><p>jdfgjdhfj dfg dfg dfg <p/>"+
                           
                           " <p> dfg df g dfg dfg df gd fgdfgdfg <p/> <h1> dfgdfgdfgdf <h1/> <p style='color: red'>dfs dfg dfg dfg df gdf gdf g<p/><span>dfgdfgdf fgghf f fg fg<span/>"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 2,
                            Question = "Question 2",
                            Answer = "<p>Ansver<p/><p> Some text jdjfhdfhdofh odhfgodhfg dhfghdfhdfh dhfod dhfig dfg dfg dfg dfgb dfg df gd fgd fgdf  dfhskdfhksdhf df df df df df df df df<p/><h1>Some text<h1/><p>jdfgjdhfj dfg dfg dfg <p/>"+
                           
                           " <p> dfg df g dfg dfg df gd fgdfgdfg <p/> <h1> dfgdfgdfgdf <h1/> <p style='color: red'>dfs dfg dfg dfg df gdf gdf g<p/><span>dfgdfgdf fgghf f fg fg<span/>"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 3,
                            Question = "Question 3",
                            Answer = "<p>Ansver<p/><p> Some text jdjfhdfhdofh odhfgodhfg dhfghdfhdfh dhfod dhfig dfg dfg dfg dfgb dfg df gd fgd fgdf <br/> dfhskdfhksdhf df df df df df df df df<p/><h1>Some text<h1/><p>jdfgjdhfj dfg dfg dfg <p/>"+
                           
                           " <p> dfg df g dfg dfg df gd fgdfgdfg <p/> <h1> dfgdfgdfgdf <h1/> <p style='color: red'>dfs dfg dfg dfg df gdf gdf g<p/><span>dfgdfgdf fgghf f fg fg<span/>"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 4,
                            Question = "Question 4",
                            Answer = "<p>Ansver<p/><p> Some text jdjfhdfhdofh odhfgodhfg dhfghdfhdfh dhfod dhfig dfg dfg dfg dfgb dfg df gd fgd fgdf <br/> dfhskdfhksdhf df df df df df df df df<p/><h1>Some text<h1/><p>jdfgjdhfj dfg dfg dfg <p/>"+
                           
                           " <p> dfg df g dfg dfg df gd fgdfgdfg <p/> <h1> dfgdfgdfgdf <h1/> <p style='color: red'>dfs dfg dfg dfg df gdf gdf g<p/><span>dfgdfgdf fgghf f fg fg<span/>"
                        },
                        new FaqEntryPartRecord
                        {
                            Id = 5,
                            Question = "Question 5",
                            Answer = "<p>Ansver<p/><p> Some text jdjfhdfhdofh odhfgodhfg dhfghdfhdfh dhfod dhfig dfg dfg dfg dfgb dfg df gd fgd fgdf <br/> dfhskdfhksdhf df df df df df df df df<p/><h1>Some text<h1/><p>jdfgjdhfj dfg dfg dfg <p/>"+
                           
                           " <p> dfg df g dfg dfg df gd fgdfgdfg <p/> <h1> dfgdfgdfgdf <h1/> <p style='color: red'>dfs dfg dfg dfg df gdf gdf g<p/><span>dfgdfgdf fgghf f fg fg<span/>"
                        },
                         new FaqEntryPartRecord
                        {
                            Id = 6,
                            Question = "Question 6",
                            Answer = "<p>Ansver<p/><p> Some text jdjfhdfhdofh odhfgodhfg dhfghdfhdfh dhfod dhfig dfg dfg dfg dfgb dfg df gd fgd fgdf <br/> dfhskdfhksdhf df df df df df df df df<p/><h1>Some text<h1/><p>jdfgjdhfj dfg dfg dfg <p/>"+
                           
                           " <p> dfg df g dfg dfg df gd fgdfgdfg <p/> <h1> dfgdfgdfgdf <h1/> <p style='color: red'>dfs dfg dfg dfg df gdf gdf g<p/><span>dfgdfgdf fgghf f fg fg<span/>"
                        },
                         new FaqEntryPartRecord
                        {
                            Id = 7,
                            Question = "Question 7",
                            Answer = "<p>Ansver<p/><p> Some text jdjfhdfhdofh odhfgodhfg dhfghdfhdfh dhfod dhfig dfg dfg dfg dfgb dfg df gd fgd fgdf <br/> dfhskdfhksdhf df df df df df df df df<p/><h1>Some text<h1/><p>jdfgjdhfj dfg dfg dfg <p/>"+
                           
                           " <p> dfg df g dfg dfg df gd fgdfgdfg <p/> <h1> dfgdfgdfgdf <h1/> <p style='color: red'>dfs dfg dfg dfg df gdf gdf g<p/><span>dfgdfgdf fgghf f fg fg<span/>"
                        },
                         new FaqEntryPartRecord
                        {
                            Id = 8,
                            Question = "Question 8",
                            Answer = "<p>Ansver<p/><p> Some text jdjfhdfhdofh odhfgodhfg dhfghdfhdfh dhfod dhfig dfg dfg dfg dfgb dfg df gd fgd fgdf <br/> dfhskdfhksdhf df df df df df df df df<p/><h1>Some text<h1/><p>jdfgjdhfj dfg dfg dfg <p/>"+
                           
                           " <p> dfg df g dfg dfg df gd fgdfgdfg <p/> <h1> dfgdfgdfgdf <h1/> <p style='color: red'>dfs dfg dfg dfg df gdf gdf g<p/><span>dfgdfgdf fgghf f fg fg<span/>"
                        }
                    }                              
            };
            return record;

         }
    }

}