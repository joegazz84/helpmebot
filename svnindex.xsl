<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"> 
	<xsl:output method="html"/> 

	<xsl:template match="svn">
		<html lang="en"> 
			<head> 
				<meta charset="UTF-8" /> 
				<title><xsl:value-of select="index/@path"/> - Subversion - Helpmebot</title> 
				<link rel="shortcut icon" href="favi	con.ico" /> 
				<link rel="copyright" href="http://creativecommons.org/licenses/by-sa/3.0/" /> 
				<link rel="stylesheet" href="http://helpmebot.org.uk/w/skins/common/shared.css" media="screen" /> 
				<link rel="stylesheet" href="http://helpmebot.org.uk/w/skins/modern/main.css" media="screen" />
				<link rel="stylesheet" href="http://helpmebot.org.uk/w/index.php?title=MediaWiki:Common.css&amp;action=raw&amp;maxage=18000&amp;usemsgcache=yes&amp;ctype=text%2Fcss&amp;smaxage=18000" /> 
				<link rel="stylesheet" href="http://helpmebot.org.uk/w/index.php?title=MediaWiki:Modern.css&amp;action=raw&amp;maxage=18000&amp;usemsgcache=yes&amp;ctype=text%2Fcss&amp;smaxage=18000" />  
			</head> 
			<body class="mediawiki ltr ns--1 ns-special page-Special_Code_helpmebot skin-modern">
				<div id="mw_header"><h1 id="firstHeading">Subversion repositories</h1></div> 
				<div id="mw_main"> 
					<div id="mw_contentwrapper"> 
						<div id="p-cactions" class="portlet"> 
							<h5>Views</h5> 
							<div class="pBody"> 
								<ul> 
									 <li id="ca-nstab-special" class="selected"><a href="#">Subversion</a></li>
								</ul> 
							</div> 
						</div> 
						<div id="mw_content"> 
							<div id="mw_contentholder" > 
								<div class='mw-topboxes'> 
									<div id="mw-js-message" style="display:none;"></div> 
								</div>
								
								<xsl:apply-templates/>
						 
								<div class='mw_clear'></div> 
							</div><!-- mw_contentholder --> 
						</div><!-- mw_content --> 
					</div><!-- mw_contentwrapper --> 
				 
					<div id="mw_portlets"> 
						<div class='generated-sidebar portlet' id='p-navigation'> 
							<h5>Links</h5> 
							<div class='pBody'> 
								<ul> 
									<li id="n-mainpage-description"><a href="http://helpmebot.org.uk/wiki/Main_Page" title="Main Page">Main Page</a></li>
									<li><a href="/">Other repositories</a></li> 
								</ul> 
							</div> 
						</div> 
					</div><!-- mw_portlets --> 
			 	</div><!-- main --> 
				<div class="mw_clear"></div> 
				<div class="portlet" id="p-personal"> 
					<h5>Personal tools</h5> 
					<div class="pBody">
						<ul>
							<li>Base: <a><xsl:value-of select="index/@base"/></a></li>
							<li>r<a><xsl:value-of select="index/@rev"/></a></li>
							<li>Path: <a><xsl:value-of select="index/@path"/></a></li>
						</ul>
					</div> 
				</div> 
				<div id="footer"> 
					<div class='mw_poweredby'>
						<xsl:element name="a"><xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute><xsl:text>Subversion</xsl:text></xsl:element><xsl:text></xsl:text><xsl:value-of select="@version"/> 
					</div>
				</div> 
			</body>
		</html> 
	</xsl:template>
	
	<xsl:template match="index">
		<ul>
			<xsl:apply-templates select="updir"/> 
			<xsl:apply-templates select="dir"/> 
			<xsl:apply-templates select="file"/> 
		</ul>
	</xsl:template>
	
	<xsl:template match="updir">
		<li>
			<div class="updir"> 
				<xsl:text>[</xsl:text> 
				<xsl:element name="a"> 
					<xsl:attribute name="href">..</xsl:attribute> 
					<xsl:text>Parent Directory</xsl:text> 
				</xsl:element> 
				<xsl:text>]</xsl:text> 
			</div>
		</li> 
	</xsl:template> 
	
	<xsl:template match="dir"> 
		<li>
			<div class="dir"> 
				<xsl:element name="a"> 
					<xsl:attribute name="href"> 
						<xsl:value-of select="@href"/> 
					</xsl:attribute> 
					<xsl:value-of select="@name"/> 
					<xsl:text>/</xsl:text> 
				</xsl:element> 
			</div> 
		</li>
	</xsl:template> 
	
	<xsl:template match="file"> 
		<li>
			<div class="file"> 
				<xsl:element name="a"> 
					<xsl:attribute name="href"> 
						<xsl:value-of select="@href"/> 
					</xsl:attribute> 
					<xsl:value-of select="@name"/> 
				</xsl:element> 
			</div> 
		</li>
	</xsl:template> 
	
	<xsl:template match="warning"> 
		<div class="warning"> 
			<xsl:value-of select="@text"/> 
		</div> 
	</xsl:template> 


</xsl:stylesheet> 