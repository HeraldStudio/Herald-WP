using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
namespace Herald
{
    [DataContract]
    public class Curriculum
    {
        [DataMember(Order = 0)]
        public WeekCurriculum content { get; set; }
        [DataMember(Order = 1)]
        public int code { get; set; }
    }
    [DataContract]
    public class WeekCurriculum
    {
        [DataMember(Order = 0)]
        public string[][] Mon { get; set; }
        [DataMember(Order = 1)]
        public string[][] Tue { get; set; }
        [DataMember(Order = 2)]
        public string[][] Wed { get; set; }
        [DataMember(Order = 3)]
        public string[][] Thu { get; set; }
        [DataMember(Order = 4)]
        public string[][] Fri { get; set; }
        [DataMember(Order = 5)]
        public string[][] Sat { get; set; }
        [DataMember(Order = 6)]
        public string[][] Sun { get; set; }
    }
    [DataContract]
    public class gpa
    {
        [DataMember(Order = 0)]
        public GPAItem[] content { get; set; }
        [DataMember(Order = 1)]
        public int code { get; set; }
    }
    //
    [DataContract]
    public class GPAItem
    {
        [DataMember(Order = 0)]
        public string gpa { get; set; }
        [DataMember(Order = 1, Name = "gpa without revamp")]
        public string gpaWithoutRevamp { get; set; }
        [DataMember(Order = 2, Name = "calculate time")]
        public string calculateTime { get; set; }
        [DataMember(Order = 3)]
        public string semester { get; set; }
        [DataMember(Order = 4)]
        public string name { get; set; }
        [DataMember(Order = 5)]
        public string credit { get; set; }
        [DataMember(Order = 6)]
        public string score { get; set; }
        [DataMember(Order = 7)]
        public string type { get; set; }
        [DataMember(Order = 8)]
        public string extra { get; set; }
    }
    [DataContract]
    public class pe
    {
        [DataMember(Order = 0)]
        public string content { get; set; }
        [DataMember(Order = 1)]
        public int code { get; set; }
    }
    [DataContract]
    public class NIC
    {
        [DataMember(Order = 0)]
        public NetItems content { get; set; }
        [DataMember(Order = 1)]
        public int code { get; set; }
    }
    [DataContract]
    public class NetItems
    {
        [DataMember(Order = 0)]
        public NetItem a { get; set; }
        [DataMember(Order = 1)]
        public NetItem web { get; set; }
        [DataMember(Order = 2)]
        public NetItem b { get; set; }    
        [DataMember(Order = 3)]
        public string left { get; set; }
    }
    [DataContract]
    public class NetItem
    {
        [DataMember(Order = 0)]
        public string state { get; set; }
        [DataMember(Order = 1)]
        public string used { get; set; }
    }
    [DataContract]
    public class Card
    {
        [DataMember(Order = 0)]
        public CardInfo content { set; get; }
        [DataMember(Order = 1)]
        public int code { set; get; }
    }
    [DataContract]
    public class CardInfo
    {
        [DataMember(Order = 0)]
        public string status { set; get; }
        [DataMember(Order = 1)]
        public ConsumeItem[] detial { set; get; }
        [DataMember(Order = 2)]
        public string left { set; get; }

    }
    [DataContract]
    public class ConsumeItem
    {
        [DataMember(Order = 0)]
        public string date { set; get; }
        [DataMember(Order = 1)]
        public string price { set; get; }
        [DataMember(Order = 2)]
        public string type { set; get; }
        [DataMember(Order = 3)]
        public string system { set; get; }
        [DataMember(Order = 4)]
        public string left { set; get; }
    }
    [DataContract]
    public class srtp
    {
        [DataMember(Order = 0)]
        public SRTPInfo[] content { get; set; }
        [DataMember(Order = 1)]
        public int code { get; set; }
    }
    [DataContract]
    public class SRTPInfo
    {
        [DataMember(Order = 0)]
        public string score { get; set; }
        [DataMember(Order = 1)]
        public string total { get; set; }
        [DataMember(Order = 2)]
        public string name { get; set; }
        [DataMember(Order = 3, Name = "card number")]
        public string cardNumber { get; set; }
        [DataMember(Order = 4)]
        public string credit { get; set; }
        [DataMember(Order = 5)]
        public string proportion { get; set; }
        [DataMember(Order = 6)]
        public string project { get; set; }
        [DataMember(Order = 7)]
        public string department { get; set; }
        [DataMember(Order = 8)]
        public string date { get; set; }
        [DataMember(Order = 9)]
        public string type { get; set; }
        [DataMember(Order = 10, Name = "total credit")]
        public string totalCredit { get; set; }
    }
    [DataContract]
    public class Lecture
    {
        [DataMember(Order = 0)]
        public LectureInfo content { get; set; }
        [DataMember(Order = 1)]
        public int code { get; set; }
    }
    [DataContract]
    public class LectureInfo
    {
        [DataMember(Order = 0)]
        public int count { get; set; }
        [DataMember(Order = 1)]
        public LectureItem[] detial { get; set; }
    }
    [DataContract]
    public class LectureItem
    {
        [DataMember(Order = 0)]
        public string date { get; set; }
        [DataMember(Order = 1)]
        public string place { get; set; }
    }
    [DataContract]
    public class Library
    {
        [DataMember(Order = 0)]
        public BookItem[] content { get; set; }
        [DataMember(Order = 1)]
        public int code { get; set; }
    }
    [DataContract]
    public class BookItem
    {
        [DataMember(Order = 0)]
        public string due_date { get; set; }
        [DataMember(Order = 1)]
        public string author { get; set; }
        [DataMember(Order = 2)]
        public string barcode { get; set; }
        [DataMember(Order = 3)]
        public string render_date { get; set; }
        [DataMember(Order = 4)]
        public string place { get; set; }
        [DataMember(Order = 5)]
        public string title { get; set; }
        [DataMember(Order = 6)]
        public string renew_time { get; set; }
    }
    [DataContract]
    public class Search
    {
        [DataMember(Order = 0)]
        public SearchBookResult[] content { get; set; }
        [DataMember(Order = 1)]
        public int code { get; set; }
    }
    [DataContract]
    public class SearchBookResult
    {
        [DataMember(Order = 0)]
        public string index { get; set; }
        [DataMember(Order = 1)]
        public string all { get; set; }
        [DataMember(Order = 2)]
        public string name { get; set; }
        [DataMember(Order = 3)]
        public string author { get; set; }
        [DataMember(Order = 4)]
        public string publish { get; set; }
        [DataMember(Order = 5)]
        public string type { get; set; }
        [DataMember(Order = 6)]
        public string left { get; set; }
    }
}
