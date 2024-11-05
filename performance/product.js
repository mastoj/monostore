import http from 'k6/http';
export const options = {
  vus: 100,
  duration: '30s',
};


const getRandomArticleNumber = () => {
  return articleNumbers[Math.floor(Math.random() * articleNumbers.length)];
}


export default function () {
  // Console log the count of created carts
  const id = getRandomArticleNumber();
  const parts = id.split('_');
  const [operatingChain, articleNumber] = parts;
  http.get(`http://localhost:5170/product/${operatingChain}/${articleNumber}`);
}

const articleNumbers = ["OCSEELG_209096",
  "OCSEELG_209097",
  "OCSEELG_209098",
  "OCSEELG_209099",
  "OCSEELG_20910",
  "OCSEELG_209100",
  "OCSEELG_209101",
  "OCSEELG_209102",
  "OCSEELG_209103",
  "OCSEELG_209104",
  "OCSEELG_209105",
  "OCSEELG_209106",
  "OCSEELG_209107",
  "OCSEELG_209108",
  "OCSEELG_209109",
  "OCSEELG_20911",
  "OCSEELG_209110",
  "OCSEELG_209111",
  "OCSEELG_209112",
  "OCSEELG_209113",
  "OCSEELG_209114",
  "OCSEELG_209115",
  "OCSEELG_209116",
  "OCSEELG_209117",
  "OCSEELG_209118",
  "OCSEELG_209119",
  "OCSEELG_20912",
  "OCSEELG_209120",
  "OCSEELG_209121",
  "OCSEELG_209122",
  "OCSEELG_209123",
  "OCSEELG_209124",
  "OCSEELG_209125",
  "OCSEELG_209126",
  "OCSEELG_209127",
  "OCSEELG_209128",
  "OCSEELG_209129",
  "OCSEELG_20913",
  "OCSEELG_209130",
  "OCSEELG_209131",
  "OCSEELG_209132",
  "OCSEELG_209133",
  "OCSEELG_209134",
  "OCSEELG_209135",
  "OCSEELG_209136",
  "OCSEELG_209137",
  "OCSEELG_209138",
  "OCSEELG_209139",
  "OCSEELG_20914",
  "OCSEELG_209140",
  "OCSEELG_209141",
  "OCSEELG_209142",
  "OCSEELG_209143",
  "OCSEELG_209144",
  "OCSEELG_209145",
  "OCSEELG_209146",
  "OCSEELG_209147",
  "OCSEELG_209148",
  "OCSEELG_209149",
  "OCSEELG_20915",
  "OCSEELG_209150",
  "OCSEELG_209151",
  "OCSEELG_209152",
  "OCSEELG_209153",
  "OCSEELG_209154",
  "OCSEELG_209155",
  "OCSEELG_209156",
  "OCSEELG_209157",
  "OCSEELG_209158",
  "OCSEELG_209159",
  "OCSEELG_20916",
  "OCSEELG_209160",
  "OCSEELG_209161",
  "OCSEELG_209162",
  "OCSEELG_209163",
  "OCSEELG_209164",
  "OCSEELG_209165",
  "OCSEELG_209166",
  "OCSEELG_209167",
  "OCSEELG_209168",
  "OCSEELG_209169",
  "OCSEELG_20917",
  "OCSEELG_209170",
  "OCSEELG_209171",
  "OCSEELG_209172",
  "OCSEELG_209173",
  "OCSEELG_209174",
  "OCSEELG_209175",
  "OCSEELG_209176",
  "OCSEELG_209177",
  "OCSEELG_209178",
  "OCSEELG_209179",
  "OCSEELG_20918",
  "OCSEELG_209180",
  "OCSEELG_209181",
  "OCSEELG_209182",
  "OCSEELG_209183",
  "OCSEELG_209184",
  "OCSEELG_209185",
  "OCSEELG_209186",
  "OCSEELG_209544",
  "OCSEELG_209545",
  "OCSEELG_209546",
  "OCSEELG_209547",
  "OCSEELG_209548",
  "OCSEELG_209549",
  "OCSEELG_20955",
  "OCSEELG_209550",
  "OCSEELG_209551",
  "OCSEELG_209552",
  "OCSEELG_209553",
  "OCSEELG_209554",
  "OCSEELG_209555",
  "OCSEELG_209556",
  "OCSEELG_209557",
  "OCSEELG_209558",
  "OCSEELG_209559",
  "OCSEELG_20956",
  "OCSEELG_209560",
  "OCSEELG_209561",
  "OCSEELG_209562",
  "OCSEELG_209563",
  "OCSEELG_209564",
  "OCSEELG_209565",
  "OCSEELG_209566",
  "OCSEELG_209567",
  "OCSEELG_209568",
  "OCSEELG_209569",
  "OCSEELG_20957",
  "OCSEELG_209570",
  "OCSEELG_209571",
  "OCSEELG_209572",
  "OCSEELG_209573",
  "OCSEELG_209574",
  "OCSEELG_209575",
  "OCSEELG_209576",
  "OCSEELG_209577",
  "OCSEELG_209578",
  "OCSEELG_209579",
  "OCSEELG_20958",
  "OCSEELG_209580",
  "OCSEELG_209581",
  "OCSEELG_209582",
  "OCSEELG_209583",
  "OCSEELG_209584",
  "OCSEELG_209585",
  "OCSEELG_209586",
  "OCSEELG_209587",
  "OCSEELG_209588",
  "OCSEELG_209589",
  "OCSEELG_209590",
  "OCSEELG_209591",
  "OCSEELG_209592",
  "OCSEELG_209593",
  "OCSEELG_209594",
  "OCSEELG_209595",
  "OCSEELG_209596",
  "OCSEELG_209597",
  "OCSEELG_209598",
  "OCSEELG_209599",
  "OCSEELG_209600",
  "OCSEELG_209601",
  "OCSEELG_209602",
  "OCSEELG_209603",
  "OCSEELG_209604",
  "OCSEELG_209605",
  "OCSEELG_209606",
  "OCSEELG_209607",
  "OCSEELG_209608",
  "OCSEELG_209609",
  "OCSEELG_209610",
  "OCSEELG_209611",
  "OCSEELG_209612",
  "OCSEELG_209613",
  "OCSEELG_209614",
  "OCSEELG_209615",
  "OCSEELG_209616",
  "OCSEELG_209617",
  "OCSEELG_209618",
  "OCSEELG_209619",
  "OCSEELG_209620",
  "OCSEELG_209621",
  "OCSEELG_209622",
  "OCSEELG_209623",
  "OCSEELG_209624",
  "OCSEELG_209625",
  "OCSEELG_209626",
  "OCSEELG_209627",
  "OCSEELG_209628",
  "OCSEELG_209629",
  "OCSEELG_20963",
  "OCSEELG_209630",
  "OCSEELG_209631",
  "OCSEELG_209632",
  "OCSEELG_209633",
  "OCSEELG_209634",
  "OCSEELG_209635",
  "OCSEELG_209636",
  "OCSEELG_209637",
  "OCSEELG_209638",
  "OCSEELG_209639",
  "OCSEELG_20964",
  "OCSEELG_209640",
  "OCSEELG_209641",
  "OCSEELG_209642",
  "OCSEELG_209643",
  "OCSEELG_209644",
  "OCSEELG_209645",
  "OCSEELG_209646",
  "OCSEELG_209647",
  "OCSEELG_209648",
  "OCSEELG_209649",
  "OCSEELG_209650",
  "OCSEELG_209651",
  "OCSEELG_209652",
  "OCSEELG_209653",
  "OCSEELG_209654",
  "OCSEELG_209655",
  "OCSEELG_209656",
  "OCSEELG_209657",
  "OCSEELG_209658",
  "OCSEELG_209659",
  "OCSEELG_209660",
  "OCSEELG_209661",
  "OCSEELG_209662",
  "OCSEELG_209663",
  "OCSEELG_209664",
  "OCSEELG_209665",
  "OCSEELG_209666",
  "OCSEELG_209667",
  "OCSEELG_209668",
  "OCSEELG_209669",
  "OCSEELG_209670",
  "OCSEELG_209671",
  "OCSEELG_209672",
  "OCSEELG_209673",
  "OCSEELG_209674",
  "OCSEELG_209675",
  "OCSEELG_209676",
  "OCSEELG_209677",
  "OCSEELG_209678",
  "OCSEELG_209679",
  "OCSEELG_20968",
  "OCSEELG_209680",
  "OCSEELG_209681",
  "OCSEELG_209682",
  "OCSEELG_209683",
  "OCSEELG_209684",
  "OCSEELG_209685",
  "OCSEELG_209686",
  "OCSEELG_209687",
  "OCSEELG_209688",
  "OCSEELG_209689",
  "OCSEELG_20969",
  "OCSEELG_209690",
  "OCSEELG_209691",
  "OCSEELG_209692",
  "OCSEELG_209693",
  "OCSEELG_209694",
  "OCSEELG_209695",
  "OCSEELG_209696",
  "OCSEELG_209697",
  "OCSEELG_209698",
  "OCSEELG_209699",
  "OCSEELG_20970",
  "OCSEELG_209700",
  "OCSEELG_209701",
  "OCSEELG_209702",
  "OCSEELG_209703",
  "OCSEELG_209704",
  "OCSEELG_209705",
  "OCSEELG_209706",
  "OCSEELG_209707",
  "OCSEELG_209708",
  "OCSEELG_209709",
  "OCSEELG_20971",
  "OCSEELG_209710",
  "OCSEELG_209711",
  "OCSEELG_209712",
  "OCSEELG_209713",
  "OCSEELG_209714",
  "OCSEELG_209715",
  "OCSEELG_209716",
  "OCSEELG_209717",
  "OCSEELG_209718",
  "OCSEELG_209719",
  "OCSEELG_20972",
  "OCSEELG_209720",
  "OCSEELG_209721",
  "OCSEELG_209722",
  "OCSEELG_209723",
  "OCSEELG_209724",
  "OCSEELG_209725",
  "OCSEELG_209726",
  "OCSEELG_209727",
  "OCSEELG_209728",
  "OCSEELG_209729",
  "OCSEELG_20973",
  "OCSEELG_209730",
  "OCSEELG_209731",
  "OCSEELG_209410",
  "OCSEELG_209411",
  "OCSEELG_209412",
  "OCSEELG_209413",
  "OCSEELG_209414",
  "OCSEELG_209415",
  "OCSEELG_209416",
  "OCSEELG_209417",
  "OCSEELG_20942",
  "OCSEELG_209428",
  "OCSEELG_209429",
  "OCSEELG_20943",
  "OCSEELG_209430",
  "OCSEELG_209431",
  "OCSEELG_209432",
  "OCSEELG_209433",
  "OCSEELG_209434",
  "OCSEELG_209435",
  "OCSEELG_209436",
  "OCSEELG_209437",
  "OCSEELG_209438",
  "OCSEELG_209439",
  "OCSEELG_209440",
  "OCSEELG_209441",
  "OCSEELG_209442",
  "OCSEELG_209443",
  "OCSEELG_209444",
  "OCSEELG_209445",
  "OCSEELG_209446",
  "OCSEELG_209447",
  "OCSEELG_209448",
  "OCSEELG_209449",
  "OCSEELG_20945",
  "OCSEELG_209450",
  "OCSEELG_209454",
  "OCSEELG_209455",
  "OCSEELG_209456",
  "OCSEELG_209457",
  "OCSEELG_209458",
  "OCSEELG_20946",
  "OCSEELG_209463",
  "OCSEELG_209464",
  "OCSEELG_209465",
  "OCSEELG_209466",
  "OCSEELG_209467",
  "OCSEELG_20947",
  "OCSEELG_209474",
  "OCSEELG_209475",
  "OCSEELG_209476",
  "OCSEELG_209477",
  "OCSEELG_209478",
  "OCSEELG_209479",
  "OCSEELG_20948",
  "OCSEELG_209480",
  "OCSEELG_209481",
  "OCSEELG_209484",
  "OCSEELG_209485",
  "OCSEELG_209486",
  "OCSEELG_209487",
  "OCSEELG_209488",
  "OCSEELG_209489",
  "OCSEELG_20949",
  "OCSEELG_209490",
  "OCSEELG_209491",
  "OCSEELG_209492",
  "OCSEELG_209493",
  "OCSEELG_209494",
  "OCSEELG_209497",
  "OCSEELG_20950",
  "OCSEELG_20951",
  "OCSEELG_209514",
  "OCSEELG_209515",
  "OCSEELG_209516",
  "OCSEELG_209517",
  "OCSEELG_209518",
  "OCSEELG_209519",
  "OCSEELG_20952",
  "OCSEELG_209520",
  "OCSEELG_209521",
  "OCSEELG_209522",
  "OCSEELG_209524",
  "OCSEELG_209526",
  "OCSEELG_209527",
  "OCSEELG_209528",
  "OCSEELG_209529",
  "OCSEELG_20953",
  "OCSEELG_209531",
  "OCSEELG_209532",
  "OCSEELG_209533",
  "OCSEELG_209534",
  "OCSEELG_209535",
  "OCSEELG_209536",
  "OCSEELG_209537",
  "OCSEELG_209538",
  "OCSEELG_209539",
  "OCSEELG_20954",
  "OCSEELG_209540",
  "OCSEELG_209541",
  "OCSEELG_209542",
  "OCSEELG_209543",
  "OCSEELG_209823",
  "OCSEELG_209824",
  "OCSEELG_209825",
  "OCSEELG_209826",
  "OCSEELG_209827",
  "OCSEELG_209828",
  "OCSEELG_209829",
  "OCSEELG_20983",
  "OCSEELG_209830",
  "OCSEELG_209831",
  "OCSEELG_209832",
  "OCSEELG_209833",
  "OCSEELG_209834",
  "OCSEELG_209835",
  "OCSEELG_209836",
  "OCSEELG_209837",
  "OCSEELG_209838",
  "OCSEELG_209839",
  "OCSEELG_20984",
  "OCSEELG_209840",
  "OCSEELG_209841",
  "OCSEELG_209842",
  "OCSEELG_209843",
  "OCSEELG_209844",
  "OCSEELG_209845",
  "OCSEELG_209846",
  "OCSEELG_209847",
  "OCSEELG_209848",
  "OCSEELG_209849",
  "OCSEELG_20985",
  "OCSEELG_209850",
  "OCSEELG_209851",
  "OCSEELG_209852",
  "OCSEELG_209855",
  "OCSEELG_209856",
  "OCSEELG_209857",
  "OCSEELG_209858",
  "OCSEELG_20986",
  "OCSEELG_209863",
  "OCSEELG_209864",
  "OCSEELG_209865",
  "OCSEELG_209866",
  "OCSEELG_209867",
  "OCSEELG_209868",
  "OCSEELG_209869",
  "OCSEELG_20987",
  "OCSEELG_209870",
  "OCSEELG_209871",
  "OCSEELG_209872",
  "OCSEELG_209873",
  "OCSEELG_209874",
  "OCSEELG_209875",
  "OCSEELG_209876",
  "OCSEELG_209877",
  "OCSEELG_209878",
  "OCSEELG_209879",
  "OCSEELG_20988",
  "OCSEELG_209880",
  "OCSEELG_209881",
  "OCSEELG_209882",
  "OCSEELG_209883",
  "OCSEELG_209884",
  "OCSEELG_209885",
  "OCSEELG_209886",
  "OCSEELG_209887",
  "OCSEELG_209888",
  "OCSEELG_209889",
  "OCSEELG_20989",
  "OCSEELG_209890",
  "OCSEELG_209891",
  "OCSEELG_209892",
  "OCSEELG_209893",
  "OCSEELG_209894",
  "OCSEELG_209895",
  "OCSEELG_209896",
  "OCSEELG_209897",
  "OCSEELG_209898",
  "OCSEELG_209899",
  "OCSEELG_20990",
  "OCSEELG_209904",
  "OCSEELG_209907",
  "OCSEELG_209908",
  "OCSEELG_20991",
  "OCSEELG_209910",
  "OCSEELG_20992",
  "OCSEELG_209922",
  "OCSEELG_209923",
  "OCSEELG_209924",
  "OCSEELG_209925",
  "OCSEELG_209926",
  "OCSEELG_209927",
  "OCSEELG_209928",
  "OCSEELG_209929",
  "OCSEELG_20993",
  "OCSEELG_209930",
  "OCSEELG_209931",
  "OCSEELG_209932",
  "OCSEELG_209933",
  "OCSEELG_209934",
  "OCSEELG_209935",
  "OCSEELG_209732",
  "OCSEELG_209733",
  "OCSEELG_209734",
  "OCSEELG_209735",
  "OCSEELG_209736",
  "OCSEELG_209737",
  "OCSEELG_209738",
  "OCSEELG_209739",
  "OCSEELG_20974",
  "OCSEELG_209740",
  "OCSEELG_209741",
  "OCSEELG_209742",
  "OCSEELG_209743",
  "OCSEELG_209744",
  "OCSEELG_209745",
  "OCSEELG_209746",
  "OCSEELG_209747",
  "OCSEELG_209748",
  "OCSEELG_209749",
  "OCSEELG_20975",
  "OCSEELG_209750",
  "OCSEELG_209751",
  "OCSEELG_209752",
  "OCSEELG_209753",
  "OCSEELG_209754",
  "OCSEELG_209755",
  "OCSEELG_209756",
  "OCSEELG_209757",
  "OCSEELG_209758",
  "OCSEELG_209759",
  "OCSEELG_20976",
  "OCSEELG_209760",
  "OCSEELG_209761",
  "OCSEELG_209762",
  "OCSEELG_209763",
  "OCSEELG_209764",
  "OCSEELG_209765",
  "OCSEELG_209766",
  "OCSEELG_209767",
  "OCSEELG_209768",
  "OCSEELG_209769",
  "OCSEELG_20977",
  "OCSEELG_209770",
  "OCSEELG_209771",
  "OCSEELG_209772",
  "OCSEELG_209773",
  "OCSEELG_209774",
  "OCSEELG_209775",
  "OCSEELG_209776",
  "OCSEELG_209777",
  "OCSEELG_209778",
  "OCSEELG_209779",
  "OCSEELG_20978",
  "OCSEELG_209780",
  "OCSEELG_209781",
  "OCSEELG_209782",
  "OCSEELG_209783",
  "OCSEELG_209784",
  "OCSEELG_209785",
  "OCSEELG_209786",
  "OCSEELG_209787",
  "OCSEELG_209788",
  "OCSEELG_209789",
  "OCSEELG_20979",
  "OCSEELG_209790",
  "OCSEELG_209791",
  "OCSEELG_209792",
  "OCSEELG_209793",
  "OCSEELG_209794",
  "OCSEELG_209795",
  "OCSEELG_209796",
  "OCSEELG_209797",
  "OCSEELG_209798",
  "OCSEELG_209799",
  "OCSEELG_20980",
  "OCSEELG_209800",
  "OCSEELG_209801",
  "OCSEELG_209802",
  "OCSEELG_209803",
  "OCSEELG_209804",
  "OCSEELG_209805",
  "OCSEELG_209806",
  "OCSEELG_209807",
  "OCSEELG_209808",
  "OCSEELG_209809",
  "OCSEELG_20981",
  "OCSEELG_209810",
  "OCSEELG_209811",
  "OCSEELG_209812",
  "OCSEELG_209813",
  "OCSEELG_209814",
  "OCSEELG_209815",
  "OCSEELG_209816",
  "OCSEELG_209817",
  "OCSEELG_209818",
  "OCSEELG_209819",
  "OCSEELG_20982",
  "OCSEELG_209820",
  "OCSEELG_209821",
  "OCSEELG_209822",
  "OCSEELG_20931",
  "OCSEELG_209311",
  "OCSEELG_209313",
  "OCSEELG_209314",
  "OCSEELG_209315",
  "OCSEELG_209316",
  "OCSEELG_209317",
  "OCSEELG_209318",
  "OCSEELG_209319",
  "OCSEELG_20932",
  "OCSEELG_209320",
  "OCSEELG_209321",
  "OCSEELG_209322",
  "OCSEELG_209323",
  "OCSEELG_209324",
  "OCSEELG_209329",
  "OCSEELG_20933",
  "OCSEELG_209330",
  "OCSEELG_209331",
  "OCSEELG_209332",
  "OCSEELG_209333",
  "OCSEELG_209334",
  "OCSEELG_209335",
  "OCSEELG_209336",
  "OCSEELG_209337",
  "OCSEELG_209338",
  "OCSEELG_209339",
  "OCSEELG_20934",
  "OCSEELG_209340",
  "OCSEELG_209341",
  "OCSEELG_209342",
  "OCSEELG_209343",
  "OCSEELG_209344",
  "OCSEELG_209345",
  "OCSEELG_209346",
  "OCSEELG_209347",
  "OCSEELG_209348",
  "OCSEELG_209349",
  "OCSEELG_20935",
  "OCSEELG_209350",
  "OCSEELG_209351",
  "OCSEELG_209352",
  "OCSEELG_209353",
  "OCSEELG_209354",
  "OCSEELG_209355",
  "OCSEELG_209356",
  "OCSEELG_209357",
  "OCSEELG_209358",
  "OCSEELG_209359",
  "OCSEELG_20936",
  "OCSEELG_209360",
  "OCSEELG_209361",
  "OCSEELG_209362",
  "OCSEELG_209363",
  "OCSEELG_209365",
  "OCSEELG_209366",
  "OCSEELG_209367",
  "OCSEELG_209368",
  "OCSEELG_209369",
  "OCSEELG_20937",
  "OCSEELG_209370",
  "OCSEELG_209371",
  "OCSEELG_209372",
  "OCSEELG_209373",
  "OCSEELG_209374",
  "OCSEELG_209375",
  "OCSEELG_209376",
  "OCSEELG_209377",
  "OCSEELG_209378",
  "OCSEELG_209379",
  "OCSEELG_20938",
  "OCSEELG_209380",
  "OCSEELG_209381",
  "OCSEELG_209382",
  "OCSEELG_209383",
  "OCSEELG_209384",
  "OCSEELG_209385",
  "OCSEELG_209386",
  "OCSEELG_209387",
  "OCSEELG_209388",
  "OCSEELG_209389",
  "OCSEELG_20939",
  "OCSEELG_209390",
  "OCSEELG_209391",
  "OCSEELG_209392",
  "OCSEELG_209393",
  "OCSEELG_209394",
  "OCSEELG_209395",
  "OCSEELG_209396",
  "OCSEELG_209397",
  "OCSEELG_209398",
  "OCSEELG_209399",
  "OCSEELG_20940",
  "OCSEELG_209400",
  "OCSEELG_209404",
  "OCSEELG_209405",
  "OCSEELG_209407",
  "OCSEELG_209408",
  "OCSEELG_209409",
  "OCSEELG_20941",
  "OCSEELG_20C600JGMN",
  "OCSEELG_20C600JJMS",
  "OCSEELG_20C600LPMN",
  "OCSEELG_20CEKAMPANJE",
  "OCSEELG_20CK003DMN",
  "OCSEELG_20DC00C8MN",
  "OCSEELG_20DF004UMN",
  "OCSEELG_20DF004XMS",
  "OCSEELG_20DF0050MN",
  "OCSEELG_20DF0093MN",
  "OCSEELG_20DF009AMN",
  "OCSEELG_20DF009AMS",
  "OCSEELG_20DL007BMN",
  "OCSEELG_20DM009KMN",
  "OCSEELG_20DM009MMN",
  "OCSEELG_20DM009NMN",
  "OCSEELG_20DM00AVMN",
  "OCSEELG_20DQ003EMS",
  "OCSEELG_20DQ003QMN",
  "OCSEELG_20DSS05M00",
  "OCSEELG_20DT0001MS",
  "OCSEELG_20DT0003MN",
  "OCSEELG_20DT001FMN",
  "OCSEELG_20DT001NMDK",
  "OCSEELG_20DT001NMN",
  "OCSEELG_20DT001TMN",
  "OCSEELG_20E2000PMS",
  "OCSEELG_20E20014MN",
  "OCSEELG_20E30012MS",
  "OCSEELG_20E3003PMN",
  "OCSEELG_20EF000RMS",
  "OCSEELG_20EF001UMN",
  "OCSEELG_20EM000RMS",
  "OCSEELG_20EM0013MN",
  "OCSEELG_20EM001AMN",
  "OCSEELG_20EM001BMN",
  "OCSEELG_20EM001BMS",
  "OCSEELG_20EN0005MN",
  "OCSEELG_20EN0007MN",
  "OCSEELG_20ER000EMS",
  "OCSEELG_20ET003HMN",
  "OCSEELG_20ET003JMN",
  "OCSEELG_20ET0047MS",
  "OCSEELG_20ET0049MS",
  "OCSEELG_20ET004LMX",
  "OCSEELG_20EV000TMS",
  "OCSEELG_20EV000UMN",
  "OCSEELG_20EV000UMS",
  "OCSEELG_20EV000XMN",
  "OCSEELG_20EV0013MD",
  "OCSEELG_20EV0013MN",
  "OCSEELG_20EV0013MS",
  "OCSEELG_20EV003AMD",
  "OCSEELG_20EV003AMX",
  "OCSEELG_20EV003EMX",
  "OCSEELG_20F1001YMN",
  "OCSEELG_20F10028MD",
  "OCSEELG_20F10028MN",
  "OCSEELG_20F10029MN",
  "OCSEELG_20F10029MS",
  "OCSEELG_20F10032MN",
  "OCSEELG_20F10038MN",
  "OCSEELG_20F5007HMN",
  "OCSEELG_20F60084MN",
  "OCSEELG_20F600A5MS",
  "OCSEELG_20F90052MN",
  "OCSEELG_20F90058MN",
  "OCSEELG_20F9005CMS",
  "OCSEELG_20F9005VMN",
  "OCSEELG_20FB002UMD",
  "OCSEELG_20FB002UMN",
  "OCSEELG_20FB003TMN",
  "OCSEELG_20FB0067MN",
  "OCSEELG_20FB0069MS",
  "OCSEELG_20FB006AMN",
  "OCSEELG_20FB006AMS",
  "OCSEELG_20FB006PMN",
  "OCSEELG_20FC0040MN",
  "OCSEELG_20FD001WMS",
  "OCSEELG_20FD001XMN",
  "OCSEELG_20FD002UMN",
  "OCSEELG_20FDMA5660",
  "OCSEELG_20FH001BMN",
  "OCSEELG_20FL000DMS",
  "OCSEELG_20FL000EMS",
  "OCSEELG_20FL000FMN",
  "OCSEELG_20FMA4760",
  "OCSEELG_20FMC5160",
  "OCSEELG_20FN003GMN",
  "OCSEELG_20FN003GMS",
  "OCSEELG_20FN004BMN",
  "OCSEELG_20FN004CMN",
  "OCSEELG_20FN004CMS",
  "OCSEELG_20FQ004WMN",
  "OCSEELG_20FQ004WMS",
  "OCSEELG_20FQ005TMN",
  "OCSEELG_20FU001LMN",
  "OCSEELG_20FU002DMN",
  "OCSEELG_20FU002DMS",
  "OCSEELG_20FU002VMN",
  "OCSEELG_209937",
  "OCSEELG_209938",
  "OCSEELG_209939",
  "OCSEELG_20994",
  "OCSEELG_209940",
  "OCSEELG_209941",
  "OCSEELG_209942",
  "OCSEELG_209943",
  "OCSEELG_209944",
  "OCSEELG_209945",
  "OCSEELG_209946",
  "OCSEELG_209947",
  "OCSEELG_209948",
  "OCSEELG_209949",
  "OCSEELG_20995",
  "OCSEELG_209950",
  "OCSEELG_209951",
  "OCSEELG_209952",
  "OCSEELG_209953",
  "OCSEELG_209954",
  "OCSEELG_209955",
  "OCSEELG_209956",
  "OCSEELG_20996",
  "OCSEELG_209965",
  "OCSEELG_209966",
  "OCSEELG_20997",
  "OCSEELG_209975",
  "OCSEELG_209976",
  "OCSEELG_209977",
  "OCSEELG_209978",
  "OCSEELG_209979",
  "OCSEELG_20998",
  "OCSEELG_209980",
  "OCSEELG_209981",
  "OCSEELG_209982",
  "OCSEELG_209983",
  "OCSEELG_209984",
  "OCSEELG_209985",
  "OCSEELG_209986",
  "OCSEELG_209987",
  "OCSEELG_20999",
  "OCSEELG_209992",
  "OCSEELG_209993",
  "OCSEELG_209994",
  "OCSEELG_209995",
  "OCSEELG_209996",
  "OCSEELG_209997",
  "OCSEELG_209998",
  "OCSEELG_20A7005KMS",
  "OCSEELG_20AA0014MS",
  "OCSEELG_20AL007SMS",
  "OCSEELG_20AL00C7MN",
  "OCSEELG_20ALA0EQMS",
  "OCSEELG_20AM006KMN",
  "OCSEELG_20AM006LMN",
  "OCSEELG_20AN009GMS",
  "OCSEELG_20AQ007SMN",
  "OCSEELG_20AQ007UMN",
  "OCSEELG_20AQ009DMN",
  "OCSEELG_20ARS04700",
  "OCSEELG_20AT004WMS",
  "OCSEELG_20AT005DMS",
  "OCSEELG_20AT005GMS",
  "OCSEELG_20AT005JMS",
  "OCSEELG_20AV0035MN",
  "OCSEELG_20AV004VMS",
  "OCSEELG_20AV006CMS",
  "OCSEELG_20AV0070MS",
  "OCSEELG_20AW0045MN",
  "OCSEELG_20AY007PMS",
  "OCSEELG_20AY00BMMS",
  "OCSEELG_20B3001VMS",
  "OCSEELG_20B3005CMS",
  "OCSEELG_20B30065MS",
  "OCSEELG_20B30077MS",
  "OCSEELG_20BE005YMS",
  "OCSEELG_20BE0086MN",
  "OCSEELG_20BE0088MN",
  "OCSEELG_20BE00BVMS",
  "OCSEELG_20BG001KMN",
  "OCSEELG_20BG0042MN",
  "OCSEELG_20BG0045MN",
  "OCSEELG_20BS0068MN",
  "OCSEELG_20BS00A7MN",
  "OCSEELG_20BS00ACMN",
  "OCSEELG_20BS00ACMS",
  "OCSEELG_20BV001JMD",
  "OCSEELG_20BV003SMN",
  "OCSEELG_20BX000TMS",
  "OCSEELG_20BX0011MN",
  "OCSEELG_20BX004KMD",
  "OCSEELG_20BX004NMN",
  "OCSEELG_20C0003SMS",
  "OCSEELG_20C1001DMS",
  "OCSEELG_20C10024MS",
  "OCSEELG_20C10027MS",
  "OCSEELG_20C10028MN",
  "OCSEELG_20C6003TMN",
  "OCSEELG_20C60046MS",
  "OCSEELG_20C600JCMS",
  "OCSEELG_209187",
  "OCSEELG_209188",
  "OCSEELG_209189",
  "OCSEELG_20919",
  "OCSEELG_209190",
  "OCSEELG_209191",
  "OCSEELG_209192",
  "OCSEELG_209193",
  "OCSEELG_209194",
  "OCSEELG_209195",
  "OCSEELG_209196",
  "OCSEELG_209197",
  "OCSEELG_209198",
  "OCSEELG_209199",
  "OCSEELG_20920",
  "OCSEELG_209200",
  "OCSEELG_209201",
  "OCSEELG_209202",
  "OCSEELG_209203",
  "OCSEELG_209204",
  "OCSEELG_209205",
  "OCSEELG_209206",
  "OCSEELG_20921",
  "OCSEELG_20922",
  "OCSEELG_20923",
  "OCSEELG_209234",
  "OCSEELG_209235",
  "OCSEELG_209236",
  "OCSEELG_209237",
  "OCSEELG_209238",
  "OCSEELG_209239",
  "OCSEELG_20924",
  "OCSEELG_209240",
  "OCSEELG_209241",
  "OCSEELG_209242",
  "OCSEELG_209243",
  "OCSEELG_209244",
  "OCSEELG_209245",
  "OCSEELG_209246",
  "OCSEELG_209247",
  "OCSEELG_209248",
  "OCSEELG_209249",
  "OCSEELG_20925",
  "OCSEELG_209250",
  "OCSEELG_209251",
  "OCSEELG_209252",
  "OCSEELG_209253",
  "OCSEELG_209254",
  "OCSEELG_209255",
  "OCSEELG_209256",
  "OCSEELG_209257",
  "OCSEELG_209258",
  "OCSEELG_209259",
  "OCSEELG_20926",
  "OCSEELG_209263",
  "OCSEELG_209264",
  "OCSEELG_209265",
  "OCSEELG_209266",
  "OCSEELG_209267",
  "OCSEELG_209268",
  "OCSEELG_209269",
  "OCSEELG_20927",
  "OCSEELG_209270",
  "OCSEELG_209271",
  "OCSEELG_209272",
  "OCSEELG_209273",
  "OCSEELG_209277",
  "OCSEELG_209278",
  "OCSEELG_209279",
  "OCSEELG_20928",
  "OCSEELG_209280",
  "OCSEELG_209281",
  "OCSEELG_209282",
  "OCSEELG_209283",
  "OCSEELG_209284",
  "OCSEELG_209285",
  "OCSEELG_209286",
  "OCSEELG_209287",
  "OCSEELG_209288",
  "OCSEELG_209289",
  "OCSEELG_20929",
  "OCSEELG_209290",
  "OCSEELG_209291",
  "OCSEELG_209292",
  "OCSEELG_209294",
  "OCSEELG_209296",
  "OCSEELG_209297",
  "OCSEELG_209298",
  "OCSEELG_209299",
  "OCSEELG_20930",
  "OCSEELG_209300",
  "OCSEELG_209301",
  "OCSEELG_209302",
  "OCSEELG_209303",
  "OCSEELG_209304",
  "OCSEELG_209305",
  "OCSEELG_209306",
  "OCSEELG_209307",
  "OCSEELG_209308",
  "OCSEELG_209309"]