#r "System.Xml.Linq"
#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/DotLiquid/lib/NET45/DotLiquid.dll"
#load "utils/dotliquid.fs"
open Suave
open Suave.Http.Successful
open Suave.Web
open Suave.Types
open System.IO
open System
open FSharp.Data

// ----------------------------------------------------------------------------
// Hello world
// ----------------------------------------------------------------------------

type News =
  { ThumbUrl : string
    LinkUrl : string
    Title : string
    Description : string }

type Weather =
  { Date : DateTime
    Icon : string
    Day : int
    Night : int }

type Home =
  { News : seq<News>
    Weather : seq<Weather> }

type Rss = XmlProvider<"http://feeds.bbci.co.uk/news/rss.xml">
type Forecast = JsonProvider<"http://api.openweathermap.org/data/2.5/forecast/daily?q=Barcelona&mode=json&units=metric&cnt=10">

let toDateTime (timestamp:int) =
  let start = DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)
  start.AddSeconds(float timestamp).ToLocalTime()

let getWeather city = 
  let f = Forecast.Load("http://api.openweathermap.org/data/2.5/forecast/daily?q=" + city + "&mode=json&units=metric&cnt=10")
  [ for it in f.List ->
      { Date = toDateTime it.Dt
        Icon = it.Weather.[0].Icon
        Day = int it.Temp.Day
        Night = int it.Temp.Night } ]

let getNews () = 
  let rss = Rss.GetSample()
  [ for it in rss.Channel.Items do
      if it.Thumbnails.Length > 0 then
        yield
          { ThumbUrl = it.Thumbnails.[0].Url
            LinkUrl = it.Link
            Title = it.Title
            Description = it.Description } ]
         
let news = { News = getNews(); Weather = getWeather("London,UK") }
let app = DotLiquid.page "index.html" news























// ----------------------------------------------------------------------------

// DEMO: Define the domain model

// TODO: Get current news from BBC
// (http://feeds.bbci.co.uk/news/rss.xml)
// TODO: Display using DotLiquid page

// TODO: Get current weather using JSON type provider
// (http://api.openweathermap.org/data/2.5/forecast/daily?q=London,UK&mode=json&units=metric&cnt=10)
// DEMO: Covert UNIX time stamps

// DEMO: Add async entry-point
// TODO: Make the data reading async

// TODO: Define NewsFilters.niceDate ('D') & add to template
// TODO: Register filters by name
