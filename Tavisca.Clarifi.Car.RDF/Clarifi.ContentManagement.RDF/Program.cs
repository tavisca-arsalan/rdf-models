﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Clarifi.ContentManagement.RDF
{
    class Program
    {
        const string BASE_URI_COUNTRY = "http://tavisca.org/Models/Country";
        static void Main(string[] args)
        {
            List<string> aliases = new List<string>();
            aliases.Add("org");
            aliases.Add("oregan");

            List<Tuple<string, string>> polygon = new List<Tuple<string, string>>();
            polygon.Add(new Tuple<string, string>("10.10", "11.11"));
            polygon.Add(new Tuple<string, string>("20.20", "22.22"));
            generateRDFForCountryMasterData("USA","East Coast","Oregon","Portland","iso1111","iso2222","ENG" ,"http://google.com","http://google.com",DateTime.Now,DateTime.Now,polygon);
            generateRDFForCurrencyMasterData("GB","Pound","GBP");
            generateRDFForStateMasterData("US","Oregon","Or",aliases,polygon);
            generateRDFForCityMasterData("Portland", "PRT", "US","ORG","Portland City",12.12,13.13,aliases,polygon);
            Console.ReadLine();
        }


        private static void generateRDFForCountryMasterData(string name,string regionName,string subRegionName, 
                                                            string intermediateRegionName, string isoCode2,
                                                            string isoCode3, string language, string flagUrl,string iconUrl,
                                                            DateTime addedDate,DateTime modifiedDate,
                                                            List<Tuple<string, string>> polygon)
        {
            //const string BASE_URI_COUNTRY = "http://tavisca.org/Models/Country";

            Graph g = new Graph();
            g.NamespaceMap.AddNamespace("clarifi", UriFactory.Create("http://tavisca.org/Clarifi/"));
            IUriNode countryNode = g.CreateUriNode(UriFactory.Create("http://tavisca.org/Models/Clarifi/Country"));

            IUriNode pName = g.CreateUriNode("clarifi:Name");
            IUriNode pRegionName = g.CreateUriNode("clarifi:Name");
            IUriNode pSubRegionName = g.CreateUriNode("clarifi:Name");
            IUriNode pIntermediateRegionName = g.CreateUriNode("clarifi:Name");
            IUriNode pISOCode2 = g.CreateUriNode("clarifi:IsoCode");
            IUriNode pISOCode3 = g.CreateUriNode("clarifi:IsoCode");        
            IUriNode pPolygon = g.CreateUriNode("clarifi:Polygon");
            IUriNode pLanguage = g.CreateUriNode("clarifi:LanguageCode");
            IUriNode pFlag = g.CreateUriNode("clarifi:Flag");
            IUriNode pIcon = g.CreateUriNode("clarifi:Icon");
            IUriNode pAddedDate = g.CreateUriNode("clarifi:AddedDate");
            IUriNode pModifiedDate = g.CreateUriNode("clarifi:ModifiedDate");

            ILiteralNode vName = g.CreateLiteralNode(name);
            ILiteralNode vRegionName = g.CreateLiteralNode(regionName);
            ILiteralNode vSubRegionName = g.CreateLiteralNode(subRegionName);
            ILiteralNode vIntermediateRegionName = g.CreateLiteralNode(intermediateRegionName);
            ILiteralNode vISOCode2 = g.CreateLiteralNode(isoCode2);
            ILiteralNode vISOCode3 = g.CreateLiteralNode(isoCode3);
            ILiteralNode vLanguage = g.CreateLiteralNode(language);
            ILiteralNode vFlag = g.CreateLiteralNode(flagUrl);
            ILiteralNode vIcon = g.CreateLiteralNode(iconUrl);
            ILiteralNode vAddedDate = g.CreateLiteralNode(addedDate.ToString());
            ILiteralNode vModifiedDate = g.CreateLiteralNode(modifiedDate.ToString());


            IBlankNode bPolygon = g.CreateBlankNode("Polygon");

            g.Assert(new Triple(countryNode, pName, vName));
            g.Assert(new Triple(countryNode, pRegionName, vRegionName));
            g.Assert(new Triple(countryNode, pSubRegionName, vSubRegionName));
            g.Assert(new Triple(countryNode, pIntermediateRegionName, vIntermediateRegionName));
            g.Assert(new Triple(countryNode, pISOCode2, vISOCode2));
            g.Assert(new Triple(countryNode, pISOCode3, vISOCode3));
            g.Assert(new Triple(countryNode, pLanguage, vLanguage));
            g.Assert(new Triple(countryNode, pFlag, vFlag));
            g.Assert(new Triple(countryNode, pIcon, vIcon));
            g.Assert(new Triple(countryNode, pAddedDate, vAddedDate));
            g.Assert(new Triple(countryNode, pModifiedDate, vModifiedDate));

            //Create a list of geo-codes for a polygon
            g.Assert(new Triple(countryNode, pPolygon, getRDFForPolygon(polygon,g,bPolygon)));
            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(g, @"C:\Users\arsalang\Desktop\CountryMaster.rdf");
        }

        public static IBlankNode getRDFForPolygon(List<Tuple<string, string>> polygon, Graph g,IBlankNode bPolygon)
        {
            IUriNode pGeoCode = g.CreateUriNode("clarifi:GeoCode");
            IUriNode pLat = g.CreateUriNode("clarifi:Lat");
            IUriNode pLong = g.CreateUriNode("clarifi:Long");
            if (polygon!=null && polygon.Count > 0)
            {
                int count = 1;
                Dictionary<string, IBlankNode> coordinatesNameDictionary = new Dictionary<string, IBlankNode>();
                foreach (var coordinates in polygon)
                {
                    coordinatesNameDictionary.Add("GeoCode" + count.ToString(), g.CreateBlankNode("GeoCodes" + count));
                    g.Assert(bPolygon, pGeoCode, coordinatesNameDictionary["GeoCode" + count.ToString()]);
                    g.Assert(coordinatesNameDictionary["GeoCode" + count.ToString()], pLat, g.CreateLiteralNode(coordinates.Item1));
                    g.Assert(coordinatesNameDictionary["GeoCode" + count.ToString()], pLong, g.CreateLiteralNode(coordinates.Item2));
                    count++;
                }
            }
            return bPolygon;
        }

        private static void generateRDFForCurrencyMasterData(string countryCode,string name, string currencyCode)
        {
            Graph g = new Graph();
            g.NamespaceMap.AddNamespace("clarifi", UriFactory.Create("http://tavisca.org/Clarifi/"));
            IUriNode currencyNode = g.CreateUriNode(UriFactory.Create("http://tavisca.org/Models/Clarifi/Currency"));

            IUriNode pCountryCode = g.CreateUriNode("clarifi:CountryCode");
            IUriNode pName = g.CreateUriNode("clarifi:CurrencyName");
            IUriNode pCurrencyCode = g.CreateUriNode("clarifi:CurrencyCode");

            ILiteralNode vCountryCode = g.CreateLiteralNode(countryCode);
            ILiteralNode vName = g.CreateLiteralNode(name);
            ILiteralNode vCurrencyCode = g.CreateLiteralNode(currencyCode);

            g.Assert(new Triple(currencyNode, pCountryCode, vCountryCode));
            g.Assert(new Triple(currencyNode, pName, vName));
            g.Assert(new Triple(currencyNode, pCurrencyCode, vCurrencyCode));

            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(g, @"C:\Users\arsalang\Desktop\CurrencyMaster.rdf");
        }

        private static void generateRDFForStateMasterData(string countryCode, string stateName, string stateCode,List<string> stateNameAliases, List<Tuple<string, string>> polygon)
        {
            Graph g = new Graph();
            g.NamespaceMap.AddNamespace("clarifi", UriFactory.Create("http://tavisca.org/Clarifi/"));
            IUriNode stateNode = g.CreateUriNode(UriFactory.Create("http://tavisca.org/Models/Clarifi/State"));

            IUriNode pCountryCode = g.CreateUriNode("clarifi:CountryCode");
            IUriNode pStateName = g.CreateUriNode("clarifi:StateName");
            IUriNode pStateCode = g.CreateUriNode("clarifi:StateCode");
            IUriNode pPolygon = g.CreateUriNode("clarifi:Polygon");
            IUriNode pStateNameAliases = g.CreateUriNode("clarifi:StateNameAliases");


            ILiteralNode vCountryCode = g.CreateLiteralNode(countryCode);
            ILiteralNode vStateName = g.CreateLiteralNode(stateName);
            ILiteralNode vStateCode = g.CreateLiteralNode(stateCode);
            ILiteralNode vStateNameAliases = g.CreateLiteralNode(getNameAliasesString(stateNameAliases));

            IBlankNode bPolygon = g.CreateBlankNode("Polygon");
            
            g.Assert(new Triple(stateNode, pCountryCode, vCountryCode));
            g.Assert(new Triple(stateNode, pStateName, vStateName));
            g.Assert(new Triple(stateNode, pStateCode, vStateCode));
            g.Assert(new Triple(stateNode, pPolygon, getRDFForPolygon(polygon, g, bPolygon)));
            g.Assert(new Triple(stateNode, pStateNameAliases, vStateNameAliases));

            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(g, @"C:\Users\arsalang\Desktop\StateMaster.rdf");
        }
        public static string getNameAliasesString(List<string> nameAliases)
        {
            String listOfAliases = "";
            if (nameAliases!=null && nameAliases.Count > 0)
            {          
                foreach (var aliasName in nameAliases)
                {
                    listOfAliases = listOfAliases + aliasName+ ',';
                }
                listOfAliases=listOfAliases.TrimEnd(',');
            }
            return listOfAliases;
        }

        private static void generateRDFForCityMasterData(string cityName, string iataCityCode, string countryCode,  string stateCode,string fullTextCode,double longitude,double latitude, List<string> cityNameAliases, List<Tuple<string, string>> polygon)
        {
            Graph g = new Graph();
            g.NamespaceMap.AddNamespace("clarifi", UriFactory.Create("http://tavisca.org/Clarifi/"));
            IUriNode cityNode = g.CreateUriNode(UriFactory.Create("http://tavisca.org/Models/Clarifi/City"));

            IUriNode pCountryCode = g.CreateUriNode("clarifi:CountryCode");
            IUriNode pCityName = g.CreateUriNode("clarifi:CityName");
            IUriNode pIATACityCode = g.CreateUriNode("clarifi:IataCityCode");
            IUriNode pStateCode = g.CreateUriNode("clarifi:StateCode");
            IUriNode pFullTextCode = g.CreateUriNode("clarifi:FullTextCode");
            IUriNode pLongitude = g.CreateUriNode("clarifi:Longitude");
            IUriNode pLatitude = g.CreateUriNode("clarifi:Latitude");
            IUriNode pPolygon = g.CreateUriNode("clarifi:Polygon");
            IUriNode pCityNameAliases = g.CreateUriNode("clarifi:CityNameAliases");

            ILiteralNode vCountryCode = g.CreateLiteralNode(countryCode);
            ILiteralNode vCityName = g.CreateLiteralNode(cityName);
            ILiteralNode vIataCityCode = g.CreateLiteralNode(iataCityCode);
            ILiteralNode vStateCode = g.CreateLiteralNode(stateCode);
            ILiteralNode vFullTextCode = g.CreateLiteralNode(fullTextCode);
            ILiteralNode vLongitude = g.CreateLiteralNode(longitude.ToString(), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDouble));
            ILiteralNode vLatitude = g.CreateLiteralNode(latitude.ToString(), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeDouble));
            ILiteralNode vCityNameAliases = g.CreateLiteralNode(getNameAliasesString(cityNameAliases));

            IBlankNode bPolygon = g.CreateBlankNode("Polygon");

            g.Assert(new Triple(cityNode, pCountryCode, vCountryCode));
            g.Assert(new Triple(cityNode, pCityName, vCityName));
            g.Assert(new Triple(cityNode, pIATACityCode, vIataCityCode));
            g.Assert(new Triple(cityNode, pStateCode, vStateCode));
            g.Assert(new Triple(cityNode, pFullTextCode, vFullTextCode));
            g.Assert(new Triple(cityNode, pLongitude, vLongitude));
            g.Assert(new Triple(cityNode, pLatitude, vLatitude));
            g.Assert(new Triple(cityNode, pPolygon, getRDFForPolygon(polygon, g, bPolygon)));
            g.Assert(new Triple(cityNode, pCityNameAliases, vCityNameAliases));

            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(g, @"C:\Users\arsalang\Desktop\CityMaster.rdf");
        }
    }
}
