<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:output method="html" indent="yes" />
    <xsl:template match="/">
        <html>
            <body>
                <h1>Statistics</h1>
                <p>Relative size of source code (total): <xsl:value-of select="//CodebaseCost"/></p>
                <p>Relative size of the source code (filtered): <xsl:value-of select="//TotalFragmentsCost"/></p>
                <p>Relative size of duplicated fragments: <xsl:value-of select="//TotalDuplicatesCost" /></p>
                <h1>Detected Duplicates</h1>
                <xsl:for-each select="//Duplicates/Duplicate">
                    <h2>Duplicated Code. Size: <xsl:value-of  select="@Cost"/></h2>
                    <h3>Duplicated Fragments:</h3>
                    <xsl:for-each select="Fragment">
                        <xsl:variable name="i" select="position()"/>
                        <p>
							Fragment <xsl:value-of select="$i"/>  in file <xsl:value-of select="FileName"/> 
							(lines <xsl:value-of select="LineRange/@Start"/>-<xsl:value-of select="LineRange/@End"/>)
						</p>
                        <pre><xsl:value-of select="Text"/></pre>
                    </xsl:for-each>
                </xsl:for-each>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>
